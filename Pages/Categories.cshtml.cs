using GroceryShopping.Data;
using GroceryShopping.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GroceryShopping.Pages;

public class CategoriesModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CategoriesModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Category> Categories { get; set; } = new();

    public async Task OnGetAsync()
    {
        Categories = await _context.Categories
            .OrderBy(c => c.Name)
            .ToListAsync();
    }
}