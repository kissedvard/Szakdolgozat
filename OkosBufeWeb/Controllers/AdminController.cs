using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using OkosBufeWeb.Data;
using Microsoft.AspNetCore.SignalR;
using OkosBufeWeb.Hubs;


// [Authorize(Roles == "Admin")]
namespace OkosBufeWeb.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<OrderHub> _hubContext;

        public AdminController(ApplicationDbContext context, IHubContext<OrderHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
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

            // Kiküldjük a hálózatra, hogy a rendelés státusza megváltozott
            await _hubContext.Clients.All.SendAsync("OrderCompleted", id);

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