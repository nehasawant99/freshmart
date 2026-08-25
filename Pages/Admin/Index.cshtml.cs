using GroceryShopping.Data;
using GroceryShopping.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GroceryShopping.Pages.Admin;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public IndexModel(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public int ProductCount { get; set; }

    public int OrderCount { get; set; }

    public int CustomerCount { get; set; }

    public decimal TotalSales { get; set; }

    public List<Order> RecentOrders { get; set; } = new();


    public async Task OnGetAsync()
    {
        // Total products
        ProductCount = await _context.Products.CountAsync();

        // Total orders
        OrderCount = await _context.Orders.CountAsync();

        // Total registered customers
        var customerUsers =
            await _userManager.GetUsersInRoleAsync("Customer");

        CustomerCount = customerUsers.Count;

        // Total sales
        TotalSales = await _context.Orders
            .SumAsync(o => o.Total);

        // Get the 5 most recent orders
        RecentOrders = await _context.Orders
            .OrderByDescending(o => o.OrderDate)
            .Take(5)
            .ToListAsync();
    }
}