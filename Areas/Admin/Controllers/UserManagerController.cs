using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopGiay.EF;
using ShopGiay.Models;

namespace ShopGiay.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles = "Admin")]
    public class UserManagerController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        ApplicationDbContext _context;
        public UserManagerController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var usersWithoutAnyRole = _context.ApplicationUsers.Where(c => !_context.UserRoles.Select(b => b.UserId).Distinct().Contains(c.Id)).ToList();
            //await _context.ApplicationUsers.ToListAsync()
            return View(usersWithoutAnyRole);
        }
        public async Task<IActionResult> Ban(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Admin/ContactsManager/Delete/5
        [HttpPost, ActionName("Ban")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ban(string id, DateTime selectedDate)
        {
            var user = await _context.ApplicationUsers.FindAsync(id);
            if (user != null)
            {
                user.LockoutEnd = selectedDate;
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> UnBan(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Admin/ContactsManager/Delete/5
        [HttpPost, ActionName("UnBan")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnBan(string id, DateTime selectedDate)
        {
            var user = await _context.ApplicationUsers.FindAsync(id);
            if (user != null)
            {
                user.LockoutEnd = null;
                _context.Update(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
