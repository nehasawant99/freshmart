using GroceryShopping.Data;
using GroceryShopping.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GroceryShopping.Pages;

[Authorize]
public class OrderConfirmationModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public OrderConfirmationModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Order? Order { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        Order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o =>
                o.Id == id &&
                o.UserId == userId);

        if (Order == null)
        {
            return NotFound();
        }

        return Page();
    }
}