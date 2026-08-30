using System.Security.Claims;
using GroceryShopping.Data;
using GroceryShopping.Models;
using GroceryShopping.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GroceryShopping.Pages;

[Authorize]
public class CheckoutModel : PageModel
{
    private readonly CartService _cartService;
    private readonly ApplicationDbContext _context;

    public CheckoutModel(
        CartService cartService,
        ApplicationDbContext context)
    {
        _cartService = cartService;
        _context = context;
    }

    public decimal Subtotal { get; set; }

    public decimal DeliveryCharge { get; set; }

    public decimal Total { get; set; }

    [BindProperty]
    public string FullName { get; set; } = string.Empty;

    [BindProperty]
    public string PhoneNumber { get; set; } = string.Empty;

    [BindProperty]
    public string Address { get; set; } = string.Empty;

    [BindProperty]
    public string City { get; set; } = string.Empty;

    [BindProperty]
    public string Pincode { get; set; } = string.Empty;


    public void OnGet()
    {
        LoadSummary();
    }


    public async Task<IActionResult> OnPostAsync()
    {
        var cartItems = _cartService.GetCart();

        if (cartItems.Count == 0)
        {
            return RedirectToPage("/Cart");
        }

        if (!ModelState.IsValid)
        {
            LoadSummary();
            return Page();
        }

        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }


        // Calculate totals

        Subtotal = _cartService.GetSubtotal();

        DeliveryCharge = Subtotal >= 500 ? 0 : 40;

        Total = Subtotal + DeliveryCharge;


        // Create Order

        var order = new Order
        {
            UserId = userId,

            OrderDate = DateTime.UtcNow,

            Subtotal = Subtotal,

            DeliveryCharge = DeliveryCharge,

            Total = Total,

            Status = "Pending",

            FullName = FullName,

            PhoneNumber = PhoneNumber,

            Address = Address,

            City = City,

            Pincode = Pincode
        };


        // Create Order Items

        foreach (var item in cartItems)
        {
            var orderItem = new OrderItem
            {
                Order = order,

                ProductId = item.ProductId,

                ProductName = item.ProductName,

                Price = item.Price,

                Quantity = item.Quantity
            };

            order.OrderItems.Add(orderItem);
        }


        // Save Order + OrderItems + Update Stock

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            _context.Orders.Add(order);

            foreach (var item in cartItems)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == item.ProductId);

                if (product == null)
                {
                    await transaction.RollbackAsync();

                    ModelState.AddModelError(
                        string.Empty,
                        $"{item.ProductName} is no longer available.");

                    LoadSummary();
                    return Page();
                }

                if (!product.IsAvailable)
                {
                    await transaction.RollbackAsync();

                    ModelState.AddModelError(
                        string.Empty,
                        $"{product.Name} is currently unavailable.");

                    LoadSummary();
                    return Page();
                }

                if (item.Quantity > product.StockQuantity)
                {
                    await transaction.RollbackAsync();

                    ModelState.AddModelError(
                        string.Empty,
                        $"Only {product.StockQuantity} unit(s) of {product.Name} are currently available.");

                    LoadSummary();
                    return Page();
                }

                // Deduct stock

                product.StockQuantity -= item.Quantity;
            }

            // Save order and updated stock

            await _context.SaveChangesAsync();

            // Commit transaction

            await transaction.CommitAsync();
        }
        catch (DbUpdateConcurrencyException)
{
    await transaction.RollbackAsync();

    ModelState.AddModelError(
        string.Empty,
        "This product was just purchased by another customer. Please review your cart and try again.");

    LoadSummary();
    return Page();
}
catch
{
    await transaction.RollbackAsync();

    ModelState.AddModelError(
        string.Empty,
        "Something went wrong while placing your order. Please try again.");

    LoadSummary();
    return Page();
}

        // Clear cart after successful order

        _cartService.ClearCart();


        // Go to order confirmation

        return RedirectToPage(
            "/OrderConfirmation",
            new { id = order.Id });
    }


    private void LoadSummary()
    {
        Subtotal = _cartService.GetSubtotal();

        if (Subtotal == 0 || Subtotal >= 500)
        {
            DeliveryCharge = 0;
        }
        else
        {
            DeliveryCharge = 40;
        }

        Total = Subtotal + DeliveryCharge;
    }
}
