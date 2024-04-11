using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopGiay.EF;
using ShopGiay.Helpers;
using ShopGiay.Models;
using ShopGiay.Repositories;

namespace ShopGiay.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;

        public ShoppingCartController(IProductRepository productRepository, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _productRepository = productRepository;
            _userManager = userManager;
            _context = context;
        }
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            // phương thức lấy thông tin sản phẩm từ productId
            Product product = await GetProductFromDatabaseAsync(productId);
            var cartItem = new CartItem
            {
                ProductId = productId,
                Name = product.Name,
                Price = product.Price,
                Quantity = quantity,
                Image = product.ImageUrl,
            };
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            cart.AddItem(cartItem);
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return RedirectToAction(nameof(Index), nameof(Product));
        }


        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            if (cart == null)
            {
                return NotFound();
            }
            cart.RemoveItem(productId);
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            cart.UpdateQuantity(productId, quantity);
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Checkout()
        {
            return View(new Order());
        }
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            return View(cart);
        }
        // Các actions khác...
        private async Task<Product> GetProductFromDatabaseAsync(int productId)
        {
            // Truy vấn cơ sở dữ liệu để lấy thông tin sản phẩm
            return await _productRepository.GetByIdAsync(productId);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Checkout(Order order)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart == null || !cart.Items.Any())
            {
                // Xử lý giỏ hàng trống...
                return RedirectToAction(nameof(Index));
            }
            var user = await _userManager.GetUserAsync(User);
            order.UserId = user.Id;
            order.OrderDate = DateTime.UtcNow;
            order.Total = cart.Items.Sum(i => i.Price * i.Quantity);
            order.OrderDetails = cart.Items.Select(i => new OrderDetail
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList();
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            HttpContext.Session.Remove("Cart");
            return View("OrderCompleted", order.Id); // Trang xác nhận hoàn thành đơn hàng
        }

        [HttpGet]
        public IActionResult CartSummary()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            var totalQuantity = cart.Items.Sum(item => item.Quantity);
            var totalPrice = cart.Items.Sum(item => item.Price * item.Quantity);

            var cartSummary = new CartModel
            {
                TotalQuantity = totalQuantity,
                TotalPrice = totalPrice
            };

            return PartialView("_CartSummary", cartSummary);
        }
    }
}

