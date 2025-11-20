using Microsoft.EntityFrameworkCore;
using PROG6212_POE_PART3.Models;

namespace PROG6212_POE_PART3.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Claim> Claims { get; set; } = null!;
    }
}
