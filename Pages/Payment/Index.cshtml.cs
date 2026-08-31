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


    // =========================
    // LOAD PAYMENT PAGE
    // =========================

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


        // =========================
        // CHECK PAYMENT EXPIRATION
        // =========================

        if (Payment.Status == "Pending" &&
            DateTime.UtcNow >= Payment.ExpiresAt)
        {
            Payment.Status = "Expired";

            Order.Status = "Cancelled";

            await _context.SaveChangesAsync();
        }


        return Page();
    }


    // =========================
    // SUCCESSFUL PAYMENT
    // =========================

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


        var payment = order.Payment;

        if (payment == null)
        {
            return NotFound();
        }


        // =========================
        // CHECK EXPIRATION
        // =========================

        if (payment.Status == "Pending" &&
            DateTime.UtcNow >= payment.ExpiresAt)
        {
            payment.Status = "Expired";

            order.Status = "Cancelled";

            await _context.SaveChangesAsync();

            return BadRequest(
                "This payment has expired.");
        }


        // =========================
        // PREVENT DUPLICATE PAYMENT
        // =========================

        if (payment.Status == "Paid")
        {
            return RedirectToPage(
                "/OrderConfirmation",
                new { id = order.Id });
        }


        // =========================
        // VALID PAYMENT STATES
        // =========================

        if (payment.Status != "Pending" &&
            payment.Status != "Failed")
        {
            return BadRequest();
        }


        // =========================
        // SIMULATE SUCCESSFUL PAYMENT
        // =========================

        payment.Status = "Paid";

        payment.TransactionId =
            $"TXN-{Guid.NewGuid().ToString("N")[..12].ToUpper()}";

        payment.PaymentDate = DateTime.UtcNow;

        order.Status = "Confirmed";


        await _context.SaveChangesAsync();


        return RedirectToPage(
            "/OrderConfirmation",
            new { id = order.Id });
    }


    // =========================
    // SIMULATE PAYMENT FAILURE
    // =========================

    public async Task<IActionResult> OnPostFailAsync(int id)
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


        var payment = order.Payment;

        if (payment == null)
        {
            return NotFound();
        }


        // Do not allow a paid payment to become failed

        if (payment.Status == "Paid")
        {
            return RedirectToPage(
                "/OrderConfirmation",
                new { id = order.Id });
        }


        // Do not allow an expired payment to become failed

        if (payment.Status == "Expired")
        {
            return BadRequest(
                "This payment has expired.");
        }


        // =========================
        // SIMULATE FAILURE
        // =========================

        payment.Status = "Failed";

        payment.TransactionId = null;

        order.Status = "Pending";


        await _context.SaveChangesAsync();


        return RedirectToPage(
            "/Payment/Index",
            new { id = order.Id });
    }
}

