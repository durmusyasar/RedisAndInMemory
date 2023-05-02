using Microsoft.EntityFrameworkCore;

namespace RedisExampleApp.API.Models
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        { }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasData(
                new Product() { Id = 1,Name = "Laptop", Price = 20000 }, 
                new Product() { Id = 2, Name = "Mouse", Price = 1000 },
                new Product() { Id = 3, Name = "Screen", Price = 4500 }
            );
            base.OnModelCreating(modelBuilder);
        }
    }
}
