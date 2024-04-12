using Microsoft.AspNetCore.Mvc;
using ShopGiay.Helpers;
using ShopGiay.Models;

namespace ShopGiay.ViewComponents
{
    public class CartSummaryViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            return View( "Cart" ,new CartModel
            {
                TotalQuantity = cart?.Items.Sum(item => item.Quantity) ?? 0,
                TotalPrice = cart?.Items.Sum(item => item.Quantity * item.Price) ?? 0,
            });
        }
    }
}
