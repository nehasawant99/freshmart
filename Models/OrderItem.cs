using System.ComponentModel.DataAnnotations;

namespace GroceryShopping.Models;

public class OrderItem
{
    public int Id { get; set; }

    // Relationship with Order
    public int OrderId { get; set; }

    public Order? Order { get; set; }

    // Product information
    public int ProductId { get; set; }

    public Product? Product { get; set; }

    // Snapshot of product information at purchase time
    [Required]
    public string ProductName { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public decimal Total => Price * Quantity;
}