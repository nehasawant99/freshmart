using GroceryShopping.Data;
using GroceryShopping.Models;
using GroceryShopping.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GroceryShopping.Pages;

public class ProductsModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly CartService _cartService;

    public ProductsModel(
        ApplicationDbContext context,
        CartService cartService)
    {
        _context = context;
        _cartService = cartService;
    }

    public List<Product> Products { get; set; } = new();

    public int? SelectedCategoryId { get; set; }

    public async Task OnGetAsync(int? categoryId)
    {
        SelectedCategoryId = categoryId;

        var query = _context.Products
            .Include(p => p.Category)
            .Where(p => p.IsAvailable)
            .AsQueryable();

        // Filter products by category
        if (categoryId.HasValue)
        {
            query = query.Where(p =>
                p.CategoryId == categoryId.Value);
        }

        Products = await query.ToListAsync();
    }

    public async Task<IActionResult> OnPostAddToCartAsync(int productId)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null)
        {
            return NotFound();
        }

        if (!product.IsAvailable || product.StockQuantity <= 0)
        {
            return RedirectToPage();
        }

        _cartService.AddToCart(product);

        return RedirectToPage("/Cart");
    }
}