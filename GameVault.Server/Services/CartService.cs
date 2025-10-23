using GameVault.Shared.Models;
using Google.Cloud.Firestore;

namespace GameVault.Server.Services;

public class CartService
{
    private readonly IFirestoreService _firestore;
    private const string CartsCollection = "carts";

    public CartService(IFirestoreService firestore)
    {
        _firestore = firestore;
    }

    public async Task<ShoppingCart> GetCartAsync(string userId)
    {
        var cart = await _firestore.GetDocumentAsync<ShoppingCart>(CartsCollection, userId);

        if (cart == null)
        {
            cart = new ShoppingCart(userId);
            await _firestore.SetDocumentAsync(CartsCollection, userId, cart);
        }

        return cart;
    }


    public async Task<CartItem> AddToCartAsync(
        string userId,
        string listingId,
        int quantity)
    {
        var cart = await GetCartAsync(userId);

        var existingItem = cart.Items.FirstOrDefault(item => item.ListingId == listingId);

        CartItem cartItem;

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
            cartItem = existingItem;
        }
        else
        {
            cartItem = new CartItem(
                listingId,
                quantity
            );

            cart.Items.Add(cartItem);
        }

        await _firestore.SetDocumentAsync(CartsCollection, userId, cart);

        return cartItem;
    }

    public async Task<CartItem?> UpdateQuantityAsync(
        string userId,
        string ListingId,
        int newQuantity)
    {
        var cart = await GetCartAsync(userId);
        var cartItem = cart.Items.FirstOrDefault(item => item.ListingId == ListingId);

        if (newQuantity <= 0)
        {
            cart.Items.Remove(cartItem);
            await _firestore.SetDocumentAsync(CartsCollection, userId, cart);
        }

        cartItem.Quantity = newQuantity;
        await _firestore.SetDocumentAsync(CartsCollection, userId, cart);
        
        return cartItem;
    }

    public async Task RemoveFromCartAsync(string userId, string listingId)
    {
        var cart = await GetCartAsync(userId);
        var cartItem = cart.Items.FirstOrDefault(item => item.ListingId == listingId);
        
        cart.Items.Remove(cartItem);
        await _firestore.SetDocumentAsync(CartsCollection, userId, cart);
    }

    public async Task ClearCartAsync(string userId)
    {
        var cart = await GetCartAsync(userId);
        cart.Items.Clear();
        await _firestore.SetDocumentAsync(CartsCollection, userId, cart);
    }
}