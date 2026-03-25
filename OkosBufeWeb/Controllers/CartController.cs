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
}
