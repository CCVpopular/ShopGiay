using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopGiay.Helpers;
using ShopGiay.Models;
using ShopGiay.Repositories;
using X.PagedList;

namespace ShopGiay.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        public ProductController(IProductRepository productRepository,
       ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }
        // Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Search(string searchString, int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 9;
            var products = string.IsNullOrEmpty(searchString)
                ? await _productRepository.GetAllAsync()
                : await _productRepository.GetByNameAsync(searchString);
            var pagedProducts = products.ToPagedList(pageNumber, pageSize);
            ViewBag.SearchString = searchString;
            // Trả về view hiển thị kết quả tìm kiếm
            return View("Search", pagedProducts);
            
        }
        public async Task<IActionResult> SearchCategory(string searchString1, int? page)
        {
            {
                var pageNumber = page ?? 1;
                var pageSize = 9;
                // Gọi phương thức tìm kiếm sản phẩm từ repository
                var products = await _productRepository.GetByNameCategoryAsync(searchString1);
                var pagedProducts = products.ToPagedList(pageNumber, pageSize);
                // Trả về view hiển thị kết quả tìm kiếm
                return View("Search", pagedProducts);
            }
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }        
        // Hiển thị thông tin chi tiết sản phẩm
        public async Task<IActionResult> Details(int id)
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
