using Microsoft.EntityFrameworkCore;
using BestStoreMVC.Models;

namespace BestStoreMVC.Services;



public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
    public DbSet<Product> Products { get; set; }
    }
