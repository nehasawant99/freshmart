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

    public async Task OnGetAsync()
    {
        Products = await _context.Products
            .Include(p => p.Category)
            .Where(p => p.IsAvailable)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostAddToCartAsync(int productId)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null)
        {
            return NotFound();
        }

        _cartService.AddToCart(product);

        return RedirectToPage("/Cart");
    }
}