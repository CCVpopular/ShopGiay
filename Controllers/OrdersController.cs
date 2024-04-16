using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopGiay.EF;
using ShopGiay.Models;

namespace ShopGiay.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index(string userid)
        {
            var applicationDbContext = _context.Orders.Include(o => o.User).Where( o => o.UserId == userid);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var orderDetails = await _context.OrderDetails.Include(od => od.Order).Include(od => od.Product).Where(od => od.OrderId == id).ToListAsync();
            if (orderDetails == null)
            {
                NotFound();
            }
            return View(orderDetails);
        }
    }
}
