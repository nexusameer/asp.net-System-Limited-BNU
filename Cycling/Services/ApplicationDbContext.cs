using Microsoft.EntityFrameworkCore;
namespace Cycling.Services;
using Cycling.Models;


public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<Cycle> Cycles { get; set; }
}
