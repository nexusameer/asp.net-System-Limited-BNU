using IMS.Models;
using Microsoft.EntityFrameworkCore;
namespace IMS.Services;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
        //use ctor for automatic constrcutor builder//
    }

    public DbSet<Product> Products { get; set; }
}
