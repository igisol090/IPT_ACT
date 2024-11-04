using Microsoft.EntityFrameworkCore;
using OrderingSystem.Models;

namespace OrderingSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<TShirt> TShirts { get; set; }
        public DbSet<OrderedTShirt> OrderedTShirts { get; set; } // Add this line for the ordered T-shirts

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TShirt>()
                .Property(t => t.Price)
                .HasColumnType("decimal(18, 2)"); // Adjust precision and scale as needed

            modelBuilder.Entity<OrderedTShirt>() // Add configuration for OrderedTShirt if needed
                .Property(o => o.TotalPrice)
                .HasColumnType("decimal(18, 2)"); // Ensure the TotalPrice is also configured
        }
    }
}
