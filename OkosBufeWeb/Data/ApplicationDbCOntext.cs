using Microsoft.EntityFrameworkCore;
using OkosBufeWeb.Models;

namespace OkosBufeWeb.Data;

public class ApplicationDbContext: DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<Product> Products { get; set; }
}