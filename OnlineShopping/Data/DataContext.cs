using Microsoft.EntityFrameworkCore;
using OnlineShopping.Model;
using OnlineShopping.Models;

namespace OnlineShopping.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        // Using the OnConfiguration to connect the SQL database.
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer("Server=(localDb)\\Local;Database=ShoppingDb;Trusted_Connection=true;TrustServerCertificate=true;");
        }

        public DbSet<Products> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<ProcessedCart> ProcessedCarts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships and mappings

            modelBuilder.Entity<Products>()
                .HasKey(p => p.Id); // Set Id as the primary key

            modelBuilder.Entity<Products>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd(); // Auto-generate Id on add

            modelBuilder.Entity<Inventory>()
                .HasKey(i => i.ProductId); // Use ProductId as the primary key

            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.Product)
                .WithOne()
                .HasForeignKey<Inventory>(i => i.ProductId); // Define the foreign key relationship with Products table

            // Insert initial values into the Products table
            modelBuilder.Entity<Products>().HasData(
                new Products { Id = 1, Name = "Hamburger", Price = 7.99M, Image = "./images/hamburger.jpg" },
                new Products { Id = 2, Name = "Cheeseburger", Price = 8.99M, Image = "./images/cheeseburger.jpg" },
                new Products { Id = 3, Name = "Tacos", Price = 7.99M, Image = "./images/tacos.jpg" },
                new Products { Id = 4, Name = "Chicken Sandwish", Price = 7.99M, Image = "./images/chicken-sandwich.jpg" },
                new Products { Id = 5, Name = "Fries", Price = 3.99M, Image = "./images/fries.jpg" },
                new Products { Id = 6, Name = "Cookies", Price = 3.49M, Image = "./images/cookies.jpg" },
                new Products { Id = 7, Name = "Soda", Price = 1.99M, Image = "./images/soda.jpg" },
                new Products { Id = 8, Name = "Draft Beer", Price = 5.99M, Image = "./images/draft.jpg" }
            );

            // Insert initial values into the Inventory table
            modelBuilder.Entity<Inventory>().HasData(
                new Inventory { ProductId = 1, Quantity = 10 },
                new Inventory { ProductId = 2, Quantity = 10 },
                new Inventory { ProductId = 3, Quantity = 10 },
                new Inventory { ProductId = 4, Quantity = 10 },
                new Inventory { ProductId = 5, Quantity = 10 },
                new Inventory { ProductId = 6, Quantity = 10 },
                new Inventory { ProductId = 7, Quantity = 10 },
                new Inventory { ProductId = 8, Quantity = 10 }
            );
        }
    }
}
