using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopGiay.Helpers;
using ShopGiay.Models;
using ShopGiay.Repositories;

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
        public async Task<IActionResult> Search(string searchString)
        {
            if (searchString == null)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // Gọi phương thức tìm kiếm sản phẩm từ repository
                var products = await _productRepository.GetByNameAsync(searchString);

                // Trả về view hiển thị kết quả tìm kiếm
                return View("Search", products);
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
