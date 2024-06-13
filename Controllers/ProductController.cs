using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
        public async Task<IActionResult> SearchWithManyVaule()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            var Brand = await _productRepository.GetBrandsAsync();
            ViewBag.Brand = new SelectList(Brand, "Id", "BrandName");
            var Color = await _productRepository.GetProductColorAsync();
            ViewBag.Color = new SelectList(Color, "Id", "Color");
            var Promotion = await _productRepository.GetPromotionAsync();
            ViewBag.Promotion = new SelectList(Promotion, "Id", "DiscountPercent");
            var Sizes = await _productRepository.GetProductSizeAsync();
            ViewBag.Size = new SelectList(Sizes, "Id", "Size");
            return PartialView("_SearchWithManyVaule");
        }
        [HttpPost]
        public async Task<IActionResult> SearchWithManyVaulePost(ProductandSizeVM model,int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 9;
            var products = await _productRepository.GetProductsWithSizes();

            if (model.CategoryId.HasValue)
                products = products.Where(p => p.CategoryId == model.CategoryId.Value);

            if (model.ProductColorId.HasValue)
                products = products.Where(p => p.ProductColorId == model.ProductColorId.Value);

            if (model.BrandId.HasValue)
                products = products.Where(p => p.BrandId == model.BrandId.Value);

            if (model.SizeId.HasValue)
                products = products.Where(p => p.ProductQuantities.Any(pq => pq.ProductSizeId == model.SizeId.Value && pq.Quantity > 0));

            if (model.IsDiscount == true)
                products = products.Where(p => p.PromotionId != null);
            if (model.MinPrice >= 0 && model.MinPrice <= model.MaxPrice)
            {
                products = products.Where(p =>
                    (p.PromotionId != null ? (p.Price - (p.Price * p.Promotions.DiscountPercent / 100)) : p.Price) > model.MinPrice &&
                    (p.PromotionId != null ? (p.Price - (p.Price * p.Promotions.DiscountPercent / 100)) : p.Price) <= model.MaxPrice
                );
            }
            var results = products.ToList();
            var pagedProducts = results.ToPagedList(pageNumber, pageSize);
            return View("Search", pagedProducts);
        }
        // Hiển thị thông tin chi tiết sản phẩm
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var Sizes = await _productRepository.GetProductSizeAsync();
            var sizesWithQuantity = Sizes.Where(size => size.ProductQuantities != null && size.ProductQuantities.Any(pq => pq.Quantity > 0 && pq.ProductId == id)).ToList();
            ViewBag.Size = sizesWithQuantity;
            var firstAvailableSize = sizesWithQuantity.FirstOrDefault()?.Size ?? 0;
            ViewBag.FirstAvailableSize = firstAvailableSize;
            return View(product);
        }
        [HttpGet]
        public async Task<IActionResult> GetQuantityBySize(int size,int id)
        {
            var sizeQuantity = await _productRepository.GetQuantityBySizeAsync(size,id);
            if (sizeQuantity == null)
            {
                return Json(new { quantity = 0 });
            }
            return Json(new { quantity = sizeQuantity.Quantity });
        }
        public async Task<IActionResult> SearchSuggestions(string searchString)
        {
            var suggestions = await _productRepository.GetSuggestionsAsync(searchString);
            return PartialView("_SearchSuggestions", suggestions);
        }
        [HttpGet]
        public async Task<IActionResult> GetQuantityBySizeAndProduct(int productId, int size)
        {
            var sizeQuantity = await _productRepository.GetProductQuantityByProductIdAndBySizeIdAsync(productId, size);
            if (sizeQuantity == null)
            {
                return Json(new { quantity = 0 });
            }
            return Json(new { quantity = sizeQuantity.Quantity });
        }

    }
}
