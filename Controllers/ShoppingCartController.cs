using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using ShopGiay.EF;
using ShopGiay.Helpers;
using ShopGiay.Models;
using ShopGiay.Repositories;
using ShopGiay.Services;

namespace ShopGiay.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;
        private readonly IVnPay _vnPayService;
        public ShoppingCartController(IProductRepository productRepository, UserManager<ApplicationUser> userManager, ApplicationDbContext context, IVnPay vnPay)
        {
            _productRepository = productRepository;
            _userManager = userManager;
            _context = context;
            _vnPayService = vnPay;
        }
        public async Task<IActionResult> AddToCart(int productId, int quantity, string action)
        {
            // phương thức lấy thông tin sản phẩm từ productId
            Product product = await GetProductFromDatabaseAsync(productId);
            if (product == null)
            {
                return NotFound(); // Trả về 404 Not Found
            }
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
            // Nếu hành động là "Mua Ngay", chuyển hướng đến trang giỏ hàng
            return RedirectToAction(nameof(Index));        
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
            if (cart == null)
            {
                return NotFound();
            }
            cart.UpdateQuantity(productId, quantity);
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart == null || cart.Items.Count == 0)
            {
                // Xử lý giỏ hàng trống...
                return RedirectToAction(nameof(Index));
            }
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
        public async Task<IActionResult> Checkout(Order order, string payment = "COD")
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
			var user = await _userManager.GetUserAsync(User);
			if (payment == "Thanh toán VnPay")
            {
                var vnPayModel = new VnPaymentRequestModel
                {
                    Amount = (double)cart.Items.Sum(p =>p.Price * p.Quantity),
                    CreatedDate = DateTime.Now,
                    Description = "Đơn hàng thành công",
                    FullName = "Khách hàng",
                    OrderId = new Random().Next(100,10000),
                };
				order.UserId = user.Id;
				order.OrderDate = DateTime.UtcNow;
				order.Total = cart.Items.Sum(i => i.Price * i.Quantity);
                order.OrderStatus = OrderStatus.Đã_Thanh_Toán;
				order.OrderDetails = cart.Items.Select(i => new OrderDetail
				{
					ProductId = i.ProductId,
					Quantity = i.Quantity,
					Price = i.Price
				}).ToList();
				_context.Orders.Add(order);
				await _context.SaveChangesAsync();
				HttpContext.Session.Remove("Cart");
				return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));
            }


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
			TempData["Message"] = "Thanh toán thành công";
			return View("OrderCompleted"); // Trang xác nhận hoàn thành đơn hàng
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
        [Authorize]
        public IActionResult PaymentFail()
        {
            return View();
        }

        [Authorize]
        public IActionResult PaymentCallBack()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Lỗi thanh toán VN Pay: {response.VnPayResponseCode}";
                return RedirectToAction("PaymentFail");
            }
            // Lưu đơn hàng vô database
            
            TempData["Message"] = $"Thanh toán VNPay thành công";
            return View("OrderCompleted");
        }
        public IActionResult PaymentSuccess()
        {
            return View("OrderCompleted");
        }
    }
}


