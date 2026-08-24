using GroceryShopping.Data;
using GroceryShopping.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GroceryShopping.Pages.Account;

[Authorize]
public class ProfileModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _context;

    public ProfileModel(
        UserManager<IdentityUser> userManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = "Customer";

    public List<Order> Orders { get; set; } = new();


    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return;
        }

        Email = user.Email ?? string.Empty;

        var roles = await _userManager.GetRolesAsync(user);

        Role = roles.FirstOrDefault() ?? "Customer";


        Orders = await _context.Orders
            .Where(o => o.UserId == user.Id)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }
}