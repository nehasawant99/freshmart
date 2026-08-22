using System.Text.Json;
using GroceryShopping.Models;
using Microsoft.AspNetCore.Http;

namespace GroceryShopping.Services;

public class CartService
{
    private const string CartSessionKey = "FreshMartCart";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public CartService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ISession Session =>
        _httpContextAccessor.HttpContext!.Session;


    // Get current cart
    public List<CartItem> GetCart()
    {
        var cartJson = Session.GetString(CartSessionKey);

        if (string.IsNullOrEmpty(cartJson))
        {
            return new List<CartItem>();
        }

        return JsonSerializer.Deserialize<List<CartItem>>(cartJson)
               ?? new List<CartItem>();
    }


    // Save cart to session
    private void SaveCart(List<CartItem> cart)
    {
        var cartJson = JsonSerializer.Serialize(cart);

        Session.SetString(CartSessionKey, cartJson);
    }


    // Add product to cart
    public void AddToCart(Product product)
    {
        var cart = GetCart();

        var existingItem = cart.FirstOrDefault(
            item => item.ProductId == product.Id
        );

        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            cart.Add(new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                Image = product.ImageUrl,
                Quantity = 1
            });
        }

        SaveCart(cart);
    }


    // Increase quantity
    public void IncreaseQuantity(int productId)
    {
        var cart = GetCart();

        var item = cart.FirstOrDefault(
            x => x.ProductId == productId
        );

        if (item != null)
        {
            item.Quantity++;
        }

        SaveCart(cart);
    }


    // Decrease quantity
    public void DecreaseQuantity(int productId)
    {
        var cart = GetCart();

        var item = cart.FirstOrDefault(
            x => x.ProductId == productId
        );

        if (item == null)
        {
            return;
        }

        item.Quantity--;

        if (item.Quantity <= 0)
        {
            cart.Remove(item);
        }

        SaveCart(cart);
    }


    // Remove product completely
    public void RemoveFromCart(int productId)
    {
        var cart = GetCart();

        cart.RemoveAll(
            item => item.ProductId == productId
        );

        SaveCart(cart);
    }


    // Calculate subtotal
    public decimal GetSubtotal()
    {
        return GetCart().Sum(item => item.Total);
    }


    // Number of products in cart
    public int GetItemCount()
    {
        return GetCart().Sum(item => item.Quantity);
    }


    // Empty cart
    public void ClearCart()
    {
        Session.Remove(CartSessionKey);
    }
}