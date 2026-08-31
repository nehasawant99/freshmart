using System.ComponentModel.DataAnnotations;

namespace GroceryShopping.Models;

public class Payment
{
    public int Id { get; set; }

    // Order linked to this payment
    public int OrderId { get; set; }

    public Order? Order { get; set; }

    // Payment information
    [Required]
    public string PaymentMethod { get; set; } = string.Empty;

    // Pending, Paid, Failed
    [Required]
    public string Status { get; set; } = "Pending";

    public decimal Amount { get; set; }

    // Gateway transaction/reference ID
    public string? TransactionId { get; set; }

    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    // Expiration date for the payment session (if applicable)
    public DateTime ExpiresAt { get; set; }
}