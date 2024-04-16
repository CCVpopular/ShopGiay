using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopGiay.Models;
using ShopGiay.Repositories;
using System.Diagnostics;
using System.IO;
using X.PagedList;


namespace ShopGiay.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles ="Admin")]
    public class ProductManagerController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        public ProductManagerController(IProductRepository productRepository,ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }
        // Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index(string searchString, int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 5; // Số lượng mục trên mỗi trang

            var products = string.IsNullOrEmpty(searchString)
                ? await _productRepository.GetAllAsync()
                : await _productRepository.GetByNameAsync(searchString);

            var pagedProducts = products.ToPagedList(pageNumber, pageSize);

            ViewBag.SearchString = searchString; // Để giữ lại giá trị của ô tìm kiếm

            return View(pagedProducts);
        }
        // Hiển thị form thêm sản phẩm mới
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }
        // Xử lý thêm sản phẩm mới
        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile imageUrl, List<IFormFile> imageUrls)
        {
            if (ModelState.IsValid)
            {
                if (imageUrl != null)
                {
                    // Lưu hình ảnh đại diện
                    product.ImageUrl = await SaveImage(imageUrl);
                }
                //if (imageUrls != null)
                //{
                //    product.ImageUrls = new List<string>();
                //    foreach (var file in imageUrls)
                //    {
                //        // Lưu các hình ảnh khác
                //        product.ImageUrls.Add(await SaveImage(file));
                //    }
                //}
                await _productRepository.AddAsync(product);
                return RedirectToAction(nameof(Add));
            }
            // Nếu ModelState không hợp lệ, hiển thị form với dữ liệu đã nhập
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }
        private async Task<string> SaveImage(IFormFile image)
        {
            var savePath = Path.Combine("wwwroot/images", image.FileName); // Thay đổi đường dẫn theo cấu hình của bạn

            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/" + image.FileName; // Trả về đường dẫn tương đối
        }

        private async Task DeleteImage(string imageUrl)
        {
            if(imageUrl != null)
            {
                //var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imageUrl.TrimStart('/'));
                var imagePath = Path.Combine("wwwroot", imageUrl.TrimStart('/'));

                if (System.IO.File.Exists(imagePath))
                {
                    // Xóa tệp ảnh nếu nó tồn tại
                    System.IO.File.Delete(imagePath);
                }
            }
            

        }

        // Hiển thị form cập nhật sản phẩm
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }
        // Xử lý cập nhật sản phẩm
        [HttpPost]
        public async Task<IActionResult> Update(int id, Product product, IFormFile imageUrl, List<IFormFile> imageUrls)
        {
            if (id != product.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var productimage = await _productRepository.GetByIdAsync(id);
                await DeleteImage(productimage.ImageUrl);

                // Lưu hình ảnh mới và cập nhật URL hình ảnh trong sản phẩm
                product.ImageUrl = await SaveImage(imageUrl);
                await _productRepository.UpdateAsync(product);


                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
        // Hiển thị form xác nhận xóa sản phẩm
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        // Xử lý xóa sản phẩm
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            await DeleteImage(product.ImageUrl);
            await _productRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
    }
}
