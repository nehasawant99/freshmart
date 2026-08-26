using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GroceryShopping.Pages.Admin.Customers;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    public IndexModel(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public List<IdentityUser> Customers { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string Search { get; set; } = string.Empty;


    public async Task OnGetAsync()
    {
        var customers = await _userManager.GetUsersInRoleAsync("Customer");

        if (!string.IsNullOrWhiteSpace(Search))
        {
            var search = Search.Trim();

            customers = customers
                .Where(u =>
                    (u.Email != null &&
                     u.Email.Contains(search, StringComparison.OrdinalIgnoreCase))
                    ||
                    (u.UserName != null &&
                     u.UserName.Contains(search, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        Customers = customers
            .OrderBy(u => u.Email)
            .ToList();
    }
}