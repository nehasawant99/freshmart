using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
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


    // =========================
    // DELIVERY VALIDATION
    // =========================

    [BindProperty]
    [Required(ErrorMessage = "Please enter your full name.")]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage = "Name must be between 2 and 100 characters.")]
    [RegularExpression(
        @"^[A-Za-z]+(?:[ .'-][A-Za-z]+)*$",
        ErrorMessage = "Please enter a valid name.")]
    public string FullName { get; set; } = string.Empty;


    [BindProperty]
    [Required(ErrorMessage = "Please enter your phone number.")]
    [RegularExpression(
        @"^[6-9]\d{9}$",
        ErrorMessage = "Please enter a valid 10-digit Indian mobile number.")]
    public string PhoneNumber { get; set; } = string.Empty;


    [BindProperty]
    [Required(ErrorMessage = "Please enter your address.")]
    [StringLength(
        250,
        MinimumLength = 5,
        ErrorMessage = "Address must be between 5 and 250 characters.")]
    public string Address { get; set; } = string.Empty;


    [BindProperty]
    [Required(ErrorMessage = "Please enter your city.")]
    [StringLength(
        50,
        MinimumLength = 2,
        ErrorMessage = "City must be between 2 and 50 characters.")]
    [RegularExpression(
        @"^[A-Za-z]+(?:[ .'-][A-Za-z]+)*$",
        ErrorMessage = "Please enter a valid city.")]
    public string City { get; set; } = string.Empty;


    [BindProperty]
    [Required(ErrorMessage = "Please enter your pincode.")]
    [RegularExpression(
        @"^\d{6}$",
        ErrorMessage = "Pincode must be exactly 6 digits.")]
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


        // Validate customer information

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


        // =========================
        // CALCULATE TOTALS
        // =========================

        Subtotal = _cartService.GetSubtotal();

        DeliveryCharge = Subtotal >= 500 ? 0 : 40;

        Total = Subtotal + DeliveryCharge;


        // =========================
        // CREATE ORDER
        // =========================

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


        // =========================
        // CREATE ORDER ITEMS
        // =========================

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


        // =========================
        // SAVE ORDER + UPDATE STOCK
        // =========================

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            _context.Orders.Add(order);


            foreach (var item in cartItems)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(
                        p => p.Id == item.ProductId);


                // Product doesn't exist

                if (product == null)
                {
                    await transaction.RollbackAsync();

                    ModelState.AddModelError(
                        string.Empty,
                        $"{item.ProductName} is no longer available.");

                    LoadSummary();

                    return Page();
                }


                // Product disabled by admin

                if (!product.IsAvailable)
                {
                    await transaction.RollbackAsync();

                    ModelState.AddModelError(
                        string.Empty,
                        $"{product.Name} is currently unavailable.");

                    LoadSummary();

                    return Page();
                }


                // Not enough stock

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


            // Save order and stock changes

            await _context.SaveChangesAsync();


            // Commit transaction

            await transaction.CommitAsync();
        }


        // =========================
        // CONCURRENCY CONFLICT
        // =========================

        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync();

            ModelState.AddModelError(
                string.Empty,
                "This product was just purchased by another customer. Please review your cart and try again.");

            LoadSummary();

            return Page();
        }


        // =========================
        // GENERAL ERROR
        // =========================

        catch
        {
            await transaction.RollbackAsync();

            ModelState.AddModelError(
                string.Empty,
                "Something went wrong while placing your order. Please try again.");

            LoadSummary();

            return Page();
        }


        // =========================
        // CLEAR CART
        // =========================

        _cartService.ClearCart();


        // =========================
        // ORDER CONFIRMATION
        // =========================

        return RedirectToPage(
            "/OrderConfirmation",
            new { id = order.Id });
    }


    // =========================
    // LOAD ORDER SUMMARY
    // =========================

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

