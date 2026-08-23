using GroceryShopping.Data;
using GroceryShopping.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GroceryShopping.Pages.Admin.Products;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Product Product { get; set; } = new();

    public SelectList Categories { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        Product = product;

        await LoadCategoriesAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadCategoriesAsync();
            return Page();
        }

        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == Product.Id);

        if (product == null)
        {
            return NotFound();
        }

        product.Name = Product.Name;
        product.Description = Product.Description;
        product.Price = Product.Price;
        product.StockQuantity = Product.StockQuantity;
        product.ImageUrl = Product.ImageUrl;
        product.IsAvailable = Product.IsAvailable;
        product.CategoryId = Product.CategoryId;

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