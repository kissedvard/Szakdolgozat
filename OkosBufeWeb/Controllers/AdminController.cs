using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using OkosBufeWeb.Data;


// [Authorize(Roles == "Admin")]
namespace OkosBufeWeb.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Lekérjük a rendeléseket, amik nincsenek kész (IsCompleted == false)
            // Az Include és ThenInclude segítségével betöltjük a termékadatokat is
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => !o.IsCompleted)
                .OrderBy(o => o.OrderTime)
                .ToListAsync();

            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> CompleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                order.IsCompleted = true;
                await _context.SaveChangesAsync();

                TempData["Success"] = $"A #{id} számú rendelés sikeresen lezárva!";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> GetActiveOrdersPartial()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => !o.IsCompleted)
                .OrderBy(o => o.OrderTime)
                .ToListAsync();

            return PartialView("_OrderCards", orders);
        }
    }
}