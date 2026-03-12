using Microsoft.AspNetCore.Mvc;
using OkosBufeWeb.Data;
using OkosBufeWeb.Models;

namespace OkosBufeWeb.Controllers;

public class ProductController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        // TESZT ADATOK!!!
        if (!_context.Products.Any())
        {
            _context.Products.AddRange(
                new Product { Name = "Sajtburger", Price = 2500, Description = "Saját készítésű marhahúspogácsa", IsAvailable = true },
                new Product { Name = "Csapolt Sör (0.5l)", Price = 1200, Description = "Hétszeresen szűrt, hideg", IsAvailable = true }
            );
            _context.SaveChanges(); 
        }

        // 1. Lekérjük az összes terméket az adatbázisból egy listába
        var products = _context.Products.ToList();

        // 2. Odaadjuk a HTML Nézetnek 
        return View(products);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken] // Beépített védelem
    public IActionResult Create(Product product)
    {
        if (ModelState.IsValid)
        {
            _context.Products.Add(product);
            _context.SaveChanges(); 

            return RedirectToAction("Index");
        }
        return View(product);
    }

}