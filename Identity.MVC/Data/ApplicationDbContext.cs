using Identity.MVC.Models;
using Microsoft.EntityFrameworkCore;

namespace Identity.MVC.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }

    }
}
