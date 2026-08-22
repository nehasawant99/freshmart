using GroceryShopping.Data;
using GroceryShopping.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GroceryShopping.Pages.Admin.Products;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Product Product { get; set; } = new();

    public SelectList Categories { get; set; } = null!;

    public async Task OnGetAsync()
    {
        await LoadCategoriesAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadCategoriesAsync();
            return Page();
        }

        _context.Products.Add(Product);

        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }

    private async Task LoadCategoriesAsync()
    {
        var categories = await _context.Categories
            .OrderBy(c => c.Name)
            .ToListAsync();

        Categories = new SelectList(
            categories,
            "Id",
            "Name"
        );
    }
}