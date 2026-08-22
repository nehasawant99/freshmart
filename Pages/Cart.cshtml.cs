using GroceryShopping.Models;
using GroceryShopping.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GroceryShopping.Pages;

public class CartModel : PageModel
{
    private readonly CartService _cartService;

    public CartModel(CartService cartService)
    {
        _cartService = cartService;
    }

    public List<CartItem> CartItems { get; set; } = new();

    public decimal Subtotal { get; set; }

    public decimal DeliveryCharge { get; set; }

    public decimal Total { get; set; }

    public int ItemCount { get; set; }


    public void OnGet()
    {
        LoadCart();
    }


    public IActionResult OnPostIncrease(int productId)
    {
        _cartService.IncreaseQuantity(productId);

        return RedirectToPage();
    }


    public IActionResult OnPostDecrease(int productId)
    {
        _cartService.DecreaseQuantity(productId);

        return RedirectToPage();
    }


    public IActionResult OnPostRemove(int productId)
    {
        _cartService.RemoveFromCart(productId);

        return RedirectToPage();
    }


    public IActionResult OnPostClear()
    {
        _cartService.ClearCart();

        return RedirectToPage();
    }


    private void LoadCart()
    {
        CartItems = _cartService.GetCart();

        ItemCount = _cartService.GetItemCount();

        Subtotal = _cartService.GetSubtotal();

        // Delivery is free for orders ₹500 or more.
        // Orders below ₹500 have ₹40 delivery charge.

        if (Subtotal == 0 || Subtotal >= 500)
        {
            DeliveryCharge = 0;
        }
        else
        {
            DeliveryCharge = 40;
        }

        Total = Subtotal + DeliveryCharge;
    }
}