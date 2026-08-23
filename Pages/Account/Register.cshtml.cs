using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GroceryShopping.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    public RegisterModel(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    [BindProperty]
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;


    [BindProperty]
    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    public string Password { get; set; } = string.Empty;


    [BindProperty]
    [Required(ErrorMessage = "Please confirm your password.")]
    [DataType(DataType.Password)]
    [Compare(
        "Password",
        ErrorMessage = "Passwords do not match."
    )]
    public string ConfirmPassword { get; set; } = string.Empty;


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


        // Check whether email already exists
        var existingUser =
            await _userManager.FindByEmailAsync(Email);

        if (existingUser != null)
        {
            ModelState.AddModelError(
                "Email",
                "An account with this email already exists."
            );

            return Page();
        }


        // Create Identity user
        var user = new IdentityUser
        {
            UserName = Email,
            Email = Email
        };


        // Identity securely hashes the password
        var result =
            await _userManager.CreateAsync(user, Password);


        // Handle Identity validation errors
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description
                );
            }

            return Page();
        }


        // Registration successful
        return RedirectToPage("/Account/Login");
    }
}