using GroceryShopping.Data;
using GroceryShopping.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GroceryShopping.Pages.Admin.Orders;

[Authorize(Roles = "Admin")]
public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Order? Order { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (Order == null)
        {
            return NotFound();
        }

        return Page();
    }


    public async Task<IActionResult> OnPostUpdateStatusAsync(
    int id,
    string status)
{
    var allowedStatuses = new[]
    {
        "Pending",
        "Confirmed",
        "OutForDelivery",
        "Delivered",
        "Cancelled"
    };

    if (!allowedStatuses.Contains(status))
    {
        return BadRequest("Invalid order status.");
    }

    var order = await _context.Orders
        .FirstOrDefaultAsync(o => o.Id == id);

    if (order == null)
    {
        return NotFound();
    }

    order.Status = status;

    await _context.SaveChangesAsync();

    return RedirectToPage(
        "/Admin/Orders/Details",
        new { id = order.Id });
}
}