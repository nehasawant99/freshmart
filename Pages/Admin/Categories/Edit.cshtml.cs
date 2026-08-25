using GroceryShopping.Data;
using GroceryShopping.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GroceryShopping.Pages.Admin.Categories;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Category Category { get; set; } = new();


    public async Task<IActionResult> OnGetAsync(int id)
{
    var category = await _context.Categories
        .FirstOrDefaultAsync(c => c.Id == id);

    if (category == null)
    {
        return NotFound();
    }

    Category = category;

    return Page();
}


    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == Category.Id);

        if (category == null)
        {
            return NotFound();
        }

        category.Name = Category.Name;
        category.Description = Category.Description;

        await _context.SaveChangesAsync();

        return RedirectToPage("/Admin/Categories/Index");
    }
}