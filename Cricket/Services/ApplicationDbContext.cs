using Microsoft.EntityFrameworkCore;
namespace Cricket.Services;
using Cricket.Models;


public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<Team> Teams { get; set; }
}
