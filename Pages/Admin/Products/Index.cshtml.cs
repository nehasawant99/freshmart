using GroceryShopping.Data;
using GroceryShopping.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GroceryShopping.Pages.Admin.Products;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Product> Products { get; set; } = new();

    public async Task OnGetAsync()
    {
        Products = await _context.Products
            .Include(p => p.Category)
            .ToListAsync();
    }
}