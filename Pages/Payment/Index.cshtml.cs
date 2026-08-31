using GroceryShopping.Data;
using GroceryShopping.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GroceryShopping.Pages.Payment;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Order? Order { get; set; }

    public GroceryShopping.Models.Payment? Payment { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        Order = await _context.Orders
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o =>
                o.Id == id &&
                o.UserId == userId);

        if (Order == null)
        {
            return NotFound();
        }

        Payment = Order.Payment;

        if (Payment == null)
        {
            return NotFound();
        }

        return Page();
    }
    public async Task<IActionResult> OnPostPayAsync(int id)
{
    var userId = User.FindFirstValue(
        ClaimTypes.NameIdentifier);

    if (string.IsNullOrEmpty(userId))
    {
        return Challenge();
    }

    var order = await _context.Orders
        .Include(o => o.Payment)
        .FirstOrDefaultAsync(o =>
            o.Id == id &&
            o.UserId == userId);

    if (order == null)
    {
        return NotFound();
    }

    if (order.Payment == null)
    {
        return NotFound();
    }

    if (order.Payment.Status == "Paid")
    {
        return RedirectToPage(
            "/OrderConfirmation",
            new { id = order.Id });
    }

    // Simulate successful payment

    order.Payment.Status = "Paid";

    order.Payment.TransactionId =
        $"TXN-{Guid.NewGuid().ToString("N")[..12].ToUpper()}";

    order.Payment.PaymentDate = DateTime.UtcNow;

    order.Status = "Confirmed";

    await _context.SaveChangesAsync();

    return RedirectToPage(
        "/OrderConfirmation",
        new { id = order.Id });
}
}