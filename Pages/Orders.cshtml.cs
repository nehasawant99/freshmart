using GroceryShopping.Data;
using GroceryShopping.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GroceryShopping.Pages;

[Authorize]
public class OrdersModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public OrdersModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Order> Orders { get; set; } = new();

    // =========================
    // LOAD USER ORDERS
    // =========================

    public async Task OnGetAsync()
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return;
        }

        Orders = await _context.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }


    // =========================
    // CANCEL ORDER
    // =========================

    public async Task<IActionResult> OnPostCancelAsync(int id)
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o =>
                o.Id == id &&
                o.UserId == userId);

        if (order == null)
        {
            return NotFound();
        }

        // Only Pending or Confirmed orders can be cancelled

        if (order.Status != "Pending" &&
            order.Status != "Confirmed")
        {
            return RedirectToPage();
        }

        // Restore product stock

        foreach (var item in order.OrderItems)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p =>
                    p.Id == item.ProductId);

            if (product != null)
            {
                product.StockQuantity += item.Quantity;
            }
        }

        // Update order status

        order.Status = "Cancelled";

        await _context.SaveChangesAsync();

        return RedirectToPage();
    }
}

