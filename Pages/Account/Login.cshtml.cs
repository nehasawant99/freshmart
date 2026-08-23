using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GroceryShopping.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public LoginModel(
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }


    [BindProperty]
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;


    [BindProperty]
    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = string.Empty;


    [BindProperty]
    public bool RememberMe { get; set; }


    public string? ErrorMessage { get; set; }


    public void OnGet()
    {
    }


    public async Task<IActionResult> OnPostAsync()
    {
        // Server-side validation
        if (!ModelState.IsValid)
        {
            return Page();
        }


        // Find user by email
        var user =
            await _userManager.FindByEmailAsync(Email);


        // Do not reveal whether email exists
        if (user == null)
        {
            ErrorMessage = "Invalid email or password.";
            return Page();
        }


        // Login through ASP.NET Identity
        var result =
            await _signInManager.PasswordSignInAsync(
                user,
                Password,
                RememberMe,
                lockoutOnFailure: true
            );


        // Successful login
        if (result.Succeeded)
        {
            // Admin → Admin panel
            if (await _userManager.IsInRoleAsync(
                    user,
                    "Admin"))
            {
                return RedirectToPage(
                    "/Admin/Products/Index"
                );
            }


            // Customer → Home
            return RedirectToPage("/Index");
        }


        // Account locked
        if (result.IsLockedOut)
        {
            ErrorMessage =
                "Your account is temporarily locked. Please try again later.";

            return Page();
        }


        // Invalid credentials
        ErrorMessage =
            "Invalid email or password.";

        return Page();
    }
}