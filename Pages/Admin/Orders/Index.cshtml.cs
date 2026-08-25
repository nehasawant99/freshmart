using GroceryShopping.Data;
using GroceryShopping.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GroceryShopping.Pages.Admin.Orders;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Order> Orders { get; set; } = new();

    public string Search { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;


    public async Task OnGetAsync(
        string? search,
        string? status)
    {
        Search = search?.Trim() ?? string.Empty;
        Status = status?.Trim() ?? string.Empty;

        var query = _context.Orders
            .AsQueryable();


        // Search by Order ID
        if (!string.IsNullOrWhiteSpace(Search))
        {
            if (int.TryParse(Search, out int orderId))
            {
                query = query.Where(o => o.Id == orderId);
            }
        }


        // Filter by status
        if (!string.IsNullOrWhiteSpace(Status))
        {
            query = query.Where(o => o.Status == Status);
        }


        Orders = await query
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }
}