using Microsoft.EntityFrameworkCore;
namespace Cricket.Services;


public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

}
