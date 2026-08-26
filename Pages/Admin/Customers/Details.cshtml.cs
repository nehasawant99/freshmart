using GroceryShopping.Data;
using GroceryShopping.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GroceryShopping.Pages.Admin.Customers;

[Authorize(Roles = "Admin")]
public class DetailsModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _context;

    public DetailsModel(
        UserManager<IdentityUser> userManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public IdentityUser Customer { get; set; } = new();

    public List<Order> Orders { get; set; } = new();


    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var customer = await _userManager.FindByIdAsync(id);

        if (customer == null)
        {
            return NotFound();
        }

        Customer = customer;

        Orders = await _context.Orders
            .Where(o => o.UserId == id)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return Page();
    }
}