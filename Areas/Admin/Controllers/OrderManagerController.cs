using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopGiay.EF;
using ShopGiay.Models;

namespace ShopGiay.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles = "Admin")]
    public class OrderManagerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderManagerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/OrderManager
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Orders.Include(o => o.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/OrderManager/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var orderDetails = await _context.OrderDetails.Include(od => od.Order).Include(od => od.Product).Where(od => od.OrderId == id).ToListAsync();
            if(orderDetails == null)
            {
                NotFound();
            }
            return View(orderDetails);
        }


        // GET: Admin/OrderManager/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.OrderStatusList = Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>().ToList();
            var order = await _context.Orders.FindAsync(id);
            return View(order);
        }

        // POST: Admin/OrderManager/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ShippingAddress,IsShipped,OrderStatus,Notes")] Order order)
        {
                try
                {
                var existingOrder = await _context.Orders.FindAsync(id);
                if (existingOrder == null)
                {
                    return NotFound();
                }
                existingOrder.ShippingAddress = order.ShippingAddress;
                existingOrder.IsShipped = order.IsShipped;
                existingOrder.OrderStatus = order.OrderStatus;
                existingOrder.Notes = order.Notes;
                existingOrder.PhoneNumber = order.PhoneNumber;
                _context.Update(existingOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index));
        }

        // GET: Admin/OrderManager/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Admin/OrderManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                var orderDetail = await _context.OrderDetails.FirstOrDefaultAsync(od => od.OrderId == id);
                if (orderDetail != null)
                {
                    _context.OrderDetails.Remove(orderDetail);
                }
                _context.Orders.Remove(order);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/OrderManager/Delete/5
        public async Task<IActionResult> DeleteorderDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails.Include(o => o.Order)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // POST: Admin/OrderManager/Delete/5
        [HttpPost, ActionName("DeleteorderDetail")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedorderDetail(int id)
        {
            var orderDetail = await _context.OrderDetails.FirstOrDefaultAsync(od => od.Id == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderDetail.OrderId);
            if (order == null)
            {
                return NotFound();
            }

            order.Total -= orderDetail.Price;
            _context.Orders.Update(order);
            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    
        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
