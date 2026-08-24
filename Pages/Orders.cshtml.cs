using GroceryShopping.Data;
using GroceryShopping.Models;
using Microsoft.AspNetCore.Authorization;
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

    public async Task OnGetAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return;
        }

        Orders = await _context.Orders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }
}