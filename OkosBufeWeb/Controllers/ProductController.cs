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

            TempData["success"] = "A termék sikeresen felvéve az étlapra!";

            return RedirectToAction("Index");
        }
        return View(product);
    }

    public IActionResult Edit(int? id)
    {
        if (id == null || id == 0) return NotFound();

        var productFromDb = _context.Products.Find(id);

        if (productFromDb == null) return NotFound();

        return View(productFromDb);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Product product)
    {
        if (ModelState.IsValid)
        {
            _context.Products.Update(product); 
            _context.SaveChanges();
            
            TempData["success"] = "A termék sikeresen módosítva!"; 
            return RedirectToAction("Index");
        }
        return View(product);
    }

    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0) return NotFound();

        var productFromDb = _context.Products.Find(id);

        if (productFromDb == null) return NotFound();

        return View(productFromDb);
    }

    
    [HttpPost, ActionName("Delete")] 
    [ValidateAntiForgeryToken]
    
    public IActionResult DeletePOST(int? id)
    {
        // Törlendő elem megkeresése
        var product = _context.Products.Find(id);
        if (product == null) return NotFound();

        // Törlés
        _context.Products.Remove(product);
        _context.SaveChanges();
        
        
        TempData["success"] = "A termék sikeresen törölve az étlapról!";
        return RedirectToAction("Index");
    }
}