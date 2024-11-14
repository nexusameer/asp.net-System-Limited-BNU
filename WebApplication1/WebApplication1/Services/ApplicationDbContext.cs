namespace WebApplication1.Services;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;  

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
        
    }
    public DbSet<Product> Products { get; set; }
}