using OkosBufeWeb.Helpers;
using Microsoft.AspNetCore.Mvc;
using OkosBufeWeb.Models;
using OkosBufeWeb.Data;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using OkosBufeWeb.Hubs;
using Microsoft.EntityFrameworkCore;

namespace OkosBufeWeb.Controllers;

public class CartController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<OrderHub> _hubContext;
    public CartController(ApplicationDbContext context, IHubContext<OrderHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public IActionResult Index()
    {
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

        return View(cart);
    }

    public IActionResult Add(int id)
    {
        var product = _context.Products.Find(id);
        if (product == null)
        {
            return NotFound();
        }

        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

        var existingItem = cart.FirstOrDefault(c => c.ProductId == id);

        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            cart.Add(new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = 1
            });

            TempData["Success"] = $"{product.Name} sikeresen bekerült a kosárba!";
        }
        HttpContext.Session.SetObjectAsJson("Cart", cart);

        return RedirectToAction("Menu", "Home");
    }

    public IActionResult Remove(int id)
    {
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

        var itemToRemove = cart.FirstOrDefault(c => c.ProductId == id);

        if (itemToRemove != null)
        {
            cart.Remove(itemToRemove);

            HttpContext.Session.SetObjectAsJson("Cart", cart);
            
            TempData["Success"] = $"{itemToRemove.ProductName} sikeresen törölve lett a kosárból!";
        }

        return RedirectToAction("Index");
    }

    public IActionResult Decrease(int id)
    {
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

        var existingItem = cart.FirstOrDefault(c => c.ProductId == id);

        if (existingItem != null)
        {
            existingItem.Quantity--;

            if (existingItem.Quantity <= 0)
            {
                cart.Remove(existingItem);
            }
        }

        HttpContext.Session.SetObjectAsJson("Cart", cart);

        return RedirectToAction("Menu", "Home");
    }

    public IActionResult UpdateCartAjax(int id, string operation)
    {
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
        var existingItem = cart.FirstOrDefault(c => c.ProductId == id);

        int currentQuantity = 0;

        if (existingItem != null)
        {
            if (operation == "add")
            {
                existingItem.Quantity++;
            }
            else if (operation == "decrease")
            {
                existingItem.Quantity--;
            }

            if (existingItem.Quantity <= 0)
            {
                cart.Remove(existingItem);
                currentQuantity = 0;
            }
            else
            {
                currentQuantity = existingItem.Quantity;
            }
        }
        HttpContext.Session.SetObjectAsJson("Cart", cart);

        return Json(new {quantity = currentQuantity});
    }


    //Rendelés leadása
    public async Task<IActionResult> Checkout()
    {
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");

        if (cart == null || !cart.Any())
        {
            TempData["Error"] = "A kosarad üres, válassz valamit a büféből!";
            return RedirectToAction("Menu", "Home");
        }

        //Új rendelés
        var newOrder = new Order
        {
            OrderTime = DateTime.Now,
            IsCompleted = false,
            CustomerName = User.Identity?.Name ?? "Szurkoló",
            OrderItems = new List<OrderItem>(),
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty

        };

        //Memóriából az adatbázisba töltés
        foreach (var item in cart)
        {
            newOrder.OrderItems.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            });
        }

        //Adatbázisba mentés

        _context.Orders.Add(newOrder);
        await _context.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("UpdateOrderDisplay");

        //Kosár kiürítése
        HttpContext.Session.Remove("Cart");

        TempData["Success"] = "Sikeresen leadtad a rendelést!";

        return RedirectToAction("Success", new{orderId = newOrder.Id} );
    }

    //Sikeres rendelés
    public IActionResult Success(int orderId)
    {
        ViewBag.orderId = orderId;
        return View();
    }



    [HttpPost]
    public async Task<IActionResult> SetUsualOrder(int orderId)
    {
        // 1. Kikeressük a célzott rendelést
        var targetOrder = await _context.Orders.FindAsync(orderId);
        if (targetOrder == null) 
        {
            return NotFound();
        }

        var previousFavorites = await _context.Orders.Where(o => o.isFavorite).ToListAsync();
        
        foreach (var order in previousFavorites)
        {
            order.isFavorite = false;
        }

        // 3. Ezt a konkrét rendelést beállítjuk az egyetlen kedvencnek
        targetOrder.isFavorite = true;
        
        // 4. Mentsük a változásokat az adatbázisba
        await _context.SaveChangesAsync();

        // Visszairányítjuk a felhasználót a Profil / Rendelések oldalra
        // (Írd át a "Profile" és "Account" neveket a te pontos útvonaladra!)
        return RedirectToAction("Profile", "Account"); 
    }


    [HttpPost]
    public async Task<IActionResult> ReorderUsual(int orderId)
    {
        // 1. Megkeressük a régi rendelést, és behúzzuk a benne lévő termékeket (és a termékek adatait)
        var oldOrder = await _context.Orders
            .Include(o => o.OrderItems)     // Ha nálad az Order modellben 'Items' a neve, cseréld arra!
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (oldOrder == null) 
        {
            return RedirectToAction("Index", "Home");
        }

        // 2. Lekérjük a jelenlegi kosarat a Sessionből (vagy ha üres, létrehozunk egy új listát)
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

        // 3. Végigmegyünk a kedvenc rendelés tételein
        foreach (var oldItem in oldOrder.OrderItems)
        {
            // Megnézzük, van-e MÁR ilyen termék a jelenlegi kosárban
            var existingCartItem = cart.FirstOrDefault(c => c.ProductId == oldItem.ProductId);
            
            if (existingCartItem != null)
            {
                // Ha már benne van, hozzáadjuk a mennyiséget (így nem lesz duplikált sor a kosárban)
                existingCartItem.Quantity += oldItem.Quantity;
            }
            else
            {
                // Ha nincs benne, újként hozzáadjuk
                cart.Add(new CartItem 
                { 
                    ProductId = oldItem.ProductId, 
    
                    // Itt a varázslat: ProductName-t és Price-t kell használnunk!
                    ProductName = oldItem.Product.Name, 
                    Price = oldItem.Product.Price, 
    
                    Quantity = oldItem.Quantity
                });
            }
        }

        // 4. Visszamentjük a frissített kosarat a Sessionbe
        HttpContext.Session.SetObjectAsJson("Cart", cart);

        // 5. Átdobjuk a felhasználót a Kosár oldalra, hogy azonnal lássa és leadhasd a rendelést
        return RedirectToAction("Index", "Cart");
    }
}
