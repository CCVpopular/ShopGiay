using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
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
        public async Task<IActionResult> AddToCart(int productId, int quantity, string action, int Size)
        {
            // phương thức lấy thông tin sản phẩm từ productId
            Product product = await GetProductFromDatabaseAsync(productId);
            if (product == null)
            {
                return NotFound(); // Trả về 404 Not Found
            }
            var Sizes = await _productRepository.GetProductSizeAsync();
            ProductSize SizeP;
            if (action == "Buy-Now" || action == "Details")
            {
                SizeP = Sizes.SingleOrDefault(s => s.Size == Size);
            }
            else
            {
                var sizesWithQuantity = Sizes.Where(size => size.ProductQuantities != null && size.ProductQuantities.Any(pq => pq.Quantity > 0 && pq.ProductId == productId)).ToList();
                Size = sizesWithQuantity.FirstOrDefault()?.Size ?? 0;             
                SizeP = Sizes.SingleOrDefault(s => s.Size == Size);
            }

            var cartItem = new CartItem
            {   
                Id = productId + "-" + SizeP.Id,
                ProductId = productId,
                Name = product.Name,
                Price = product.DiscountedPrice,
                Quantity = quantity,
                Image = product.ImageUrl,
                SizeId = SizeP.Id,
            };
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            cart.AddItem(cartItem);
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            // Nếu hành động là "Mua Ngay", chuyển hướng đến trang giỏ hàng
            if (action == "Buy-Now")
            {
                return RedirectToAction(nameof(Index));
            }
            else if(action == "Details")
            {
                return RedirectToAction(nameof(Details),nameof(Product), new { id = productId });
            }
            else
            {
                return RedirectToAction(action,nameof(Product));
            }
        }

        public async Task<IActionResult> RemoveFromCart(string productId)
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

        public async Task<IActionResult> UpdateSize(string cartId,int productId , int sizeId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            if (cart == null)
            {
                return NotFound();
            }
            var existingItem = cart.Items.FirstOrDefault(ci => ci.SizeId == sizeId && ci.ProductId == productId);
            if (existingItem != null)
            {
                TempData["ErrorMessage"] = "Size này đã được chọn cho sản phẩm này.";
                return RedirectToAction(nameof(Index));
            }
            cart.UpdateSize(cartId, sizeId);
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UpdateQuantity(int productId, int quantity,string cartId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            if (cart == null)
            {
                return NotFound();
            }

            var Item = cart.Items.FirstOrDefault(ci => ci.Id == cartId);
            var product = await _context.ProductQuantities.FirstOrDefaultAsync(ci => ci.ProductId == productId && Item.SizeId == ci.ProductSizeId && ci.Quantity >= quantity);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Số lượng giày không vượt quá số lượng trong kho.";
                return RedirectToAction(nameof(Index));
            }
            cart.UpdateQuantity(productId, quantity, cartId);
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
        public async Task<IActionResult> Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            var productIds = cart.Items.Select(item => item.ProductId).ToList();
            var sizesWithQuantity = await _productRepository.GetQuantitiesForProducts(productIds);

            var sizesWithQuantityablevaule = sizesWithQuantity.Where(item => item.Quantity > 0).ToList();
            ViewBag.Size = sizesWithQuantityablevaule;
            var Sizes = await _productRepository.GetProductSizeAsync();
            ViewBag.Sizename = Sizes;
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
            foreach (var item in cart.Items)
            {
                var quantity = await _context.ProductQuantities.FirstOrDefaultAsync(p => p.ProductId == item.ProductId && p.ProductSizeId == item.SizeId);
                if (quantity != null && quantity.Quantity >= item.Quantity)
                {
                    quantity.Quantity -= item.Quantity;
                    _context.ProductQuantities.Update(quantity);
                }
            }
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
					Price = i.Price,
                    ProductSizeId = i.SizeId,
                }).ToList();
                OrderidVM orderid = new OrderidVM();
                orderid.ID = order.Id;
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
                Price = i.Price,
                ProductSizeId= i.SizeId,
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


