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

        // Define DbSet properties for each model (table)
        public DbSet<User> Users { get; set; } = null!; // Users table
        public DbSet<Claim> Claims { get; set; } = null!; // Claims table

        // This method configures the relationships between entities and additional settings
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships or additional configurations here

            modelBuilder.Entity<Claim>()
                .HasOne(c => c.Lecturer)
                .WithMany()
                .HasForeignKey(c => c.LecturerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
