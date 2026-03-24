using OkosBufeWeb.Helpers;
using Microsoft.AspNetCore.Mvc;
using OkosBufeWeb.Models;
using OkosBufeWeb.Data;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace OkosBufeWeb.Controllers;

public class CartController : Controller
{
    private readonly ApplicationDbContext _context;
    
    public CartController(ApplicationDbContext context)
    {
        _context = context;
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
        }
        HttpContext.Session.SetObjectAsJson("Cart", cart);

        TempData["Success"] = $"{product.Name} sikeresen bekerült a kosárba!";
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
}
