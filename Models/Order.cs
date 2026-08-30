using System.ComponentModel.DataAnnotations;

namespace GroceryShopping.Models;

public class Order
{
    public int Id { get; set; }

    // Customer who placed the order
    [Required]
    public string UserId { get; set; } = string.Empty;

    // Order information
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public decimal Subtotal { get; set; }

    public decimal DeliveryCharge { get; set; }

    public decimal Total { get; set; }

    // Pending, Confirmed, OutForDelivery, Delivered, Cancelled
    public string Status { get; set; } = "Pending";

    // Delivery information
    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public string Address { get; set; } = string.Empty;

    [Required]
    public string City { get; set; } = string.Empty;

    [Required]
    public string Pincode { get; set; } = string.Empty;

    // Order items
    public List<OrderItem> OrderItems { get; set; } = new();
  
    // Payment
    public Payment? Payment { get; set; }
}