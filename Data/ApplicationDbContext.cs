using GroceryShopping.Models;
using Microsoft.EntityFrameworkCore;

namespace GroceryShopping.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =========================
        // PRODUCT PRICE PRECISION
        // =========================

        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);


        // =========================
        // CATEGORIES
        // =========================

        modelBuilder.Entity<Category>().HasData(

            new Category
            {
                Id = 1,
                Name = "Fruits",
                Description = "Fresh and delicious fruits"
            },

            new Category
            {
                Id = 2,
                Name = "Vegetables",
                Description = "Fresh and healthy vegetables"
            },

            new Category
            {
                Id = 3,
                Name = "Dairy",
                Description = "Milk and dairy products"
            },

            new Category
            {
                Id = 4,
                Name = "Bakery",
                Description = "Fresh bakery products"
            },

            new Category
            {
                Id = 5,
                Name = "Snacks",
                Description = "Tasty snacks and packaged foods"
            }
        );


        // =========================
        // PRODUCTS
        // =========================

        modelBuilder.Entity<Product>().HasData(

            new Product
            {
                Id = 1,
                Name = "Fresh Apples",
                Description = "Fresh red apples",
                Price = 180,
                StockQuantity = 50,
                ImageUrl = "/images/products/apples.jpg",
                CategoryId = 1,
                IsAvailable = true
            },

            new Product
            {
                Id = 2,
                Name = "Fresh Bananas",
                Description = "Fresh ripe bananas",
                Price = 60,
                StockQuantity = 80,
                ImageUrl = "/images/products/bananas.jpg",
                CategoryId = 1,
                IsAvailable = true
            },

            new Product
            {
                Id = 3,
                Name = "Potatoes",
                Description = "Fresh potatoes",
                Price = 40,
                StockQuantity = 100,
                ImageUrl = "/images/products/potatoes.jpg",
                CategoryId = 2,
                IsAvailable = true
            },

            new Product
            {
                Id = 4,
                Name = "Fresh Milk",
                Description = "Full cream fresh milk",
                Price = 60,
                StockQuantity = 50,
                ImageUrl = "/images/products/milk.jpg",
                CategoryId = 3,
                IsAvailable = true
            },

            new Product
            {
                Id = 5,
                Name = "Brown Bread",
                Description = "Healthy brown bread",
                Price = 50,
                StockQuantity = 40,
                ImageUrl = "/images/products/bread.jpg",
                CategoryId = 4,
                IsAvailable = true
            },

            new Product
            {
                Id = 6,
                Name = "Potato Chips",
                Description = "Crispy potato chips",
                Price = 30,
                StockQuantity = 100,
                ImageUrl = "/images/products/chips.jpg",
                CategoryId = 5,
                IsAvailable = true
            }
        );
    }
}