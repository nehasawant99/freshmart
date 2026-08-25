using GroceryShopping.Data;
using GroceryShopping.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GroceryShopping.Pages.Admin.Categories;

[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Category Category { get; set; } = new();

    public int ProductCount { get; set; }


    public async Task<IActionResult> OnGetAsync(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        Category = category;

        ProductCount = category.Products.Count;

        return Page();
    }


    public async Task<IActionResult> OnPostAsync(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return NotFound();
        }


        // Do not delete categories that still contain products

        if (category.Products.Any())
        {
            TempData["DeleteSuccess"] =
                $"Category '{category.Name}' was deleted successfully.";

            return RedirectToPage("/Admin/Categories/Index");
        }


        _context.Categories.Remove(category);

        await _context.SaveChangesAsync();

        return RedirectToPage("/Admin/Categories/Index");
    }
}