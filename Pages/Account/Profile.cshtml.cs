using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GroceryShopping.Pages.Account;

[Authorize]
public class ProfileModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    public ProfileModel(
        UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = "Customer";


    public async Task OnGetAsync()
    {
        var user =
            await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return;
        }

        Email = user.Email ?? string.Empty;

        var roles =
            await _userManager.GetRolesAsync(user);

        Role = roles.FirstOrDefault() ?? "Customer";
    }
}