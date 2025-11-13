using GameVault.Server.Models.Firestore;
using GameVault.Shared.DTOs;
using GameVault.Shared.Models;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Microsoft.AspNetCore.Authorization;
using GameVault.Server.Filters;

namespace GameVault.Server.Services;

public class CartService
{
    private readonly IFirestoreService _firestore;
    private const string CartsCollection = "carts";

    public CartService(IFirestoreService firestore, ICurrentUserService currentUserService)
    {
        _firestore = firestore;
    }

    public async Task<Cart> GetCartAsync(string userId)
    {
        var result = await _firestore.GetDocumentAsync<FirestoreCart>(CartsCollection, userId);

        Cart cart;
        if (result == null)
        {
            var firestoreCart = new FirestoreCart()
            {
                Items = []
            };
            cart = new Cart()
            {
                OwnerId = userId,
                Items = []
            };
            await _firestore.SetDocumentAsync(CartsCollection, userId, firestoreCart);
        }
        else
        {
            cart = Cart.FromCart(result, userId);
        }

        return cart;
    }


    public async Task<Models.Firestore.CartItem> AddToCartAsync(
        string userId,
        string listingId,
        int quantity)
    {
        var cart = await GetCartAsync(userId);

        var existingItem = cart.Items.FirstOrDefault(item => item.ListingId == listingId);

        Models.Firestore.CartItem? cartItem;

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
            cartItem = existingItem;
        }
        else
        {
            cartItem = new Models.Firestore.CartItem()
            {
                ListingId = listingId,
                Quantity = quantity
            };

            cart.Items.Add(cartItem);
        }

        await _firestore.SetDocumentAsync(CartsCollection, userId, new FirestoreCart()
        {
            Items = cart.Items
        });

        return cartItem;
    }

    public async Task<Models.Firestore.CartItem?> UpdateQuantityAsync(
        string userId,
        string ListingId,
        int newQuantity)
    {
        var cart = await GetCartAsync(userId);
        var cartItem = cart.Items.FirstOrDefault(item => item.ListingId == ListingId);

        if (cartItem is not null)
        {
            if (newQuantity <= 0)
            {
                cart.Items.Remove(cartItem);
                await _firestore.SetDocumentAsync(CartsCollection, userId, new FirestoreCart()
                {
                    Items = cart.Items
                });
                return cartItem;
            }

            cartItem.Quantity = newQuantity;
            await _firestore.SetDocumentAsync(CartsCollection, userId, new FirestoreCart()
            {
                Items = cart.Items
            });
        }

        return cartItem;
    }

    public async Task RemoveFromCartAsync(string userId, string listingId)
    {
        var cart = await GetCartAsync(userId);
        if (cart.Items.Count > 0)
        {
            var cartItem = cart.Items.FirstOrDefault(item => item.ListingId == listingId);

            if (cartItem is not null)
            {
                cart.Items.Remove(cartItem);
                await _firestore.SetDocumentAsync(CartsCollection, userId, new FirestoreCart()
                {
                    Items = cart.Items
                });
            }
        }
    }

    public async Task ClearCartAsync(string userId)
    {
        var cart = await GetCartAsync(userId);
        cart.Items.Clear();
        await _firestore.SetDocumentAsync(CartsCollection, userId, new FirestoreCart()
        {
            Items = cart.Items
        });
    }

    public async Task<CartItemDTO> PopulateCartItem(Models.Firestore.CartItem cartItem)
    {
        // Get listing details
        // * Price
        // * Image
        // * Name
        // * VendorId
        Listing? listing = await _firestore.GetDocumentAsync<Listing>("listings", cartItem.ListingId);

        if (listing is null)
        {
            return new()
            {
                ListingId = cartItem.ListingId,
                Quantity = cartItem.Quantity,
                ListingName = "Not Found",
                PriceInCents = 0,
                ThumbnailUrl = "",
                VendorId = "",
                VendorName = "Not Found",
            };
        }

        // Get vendor name
        var vendor = await _firestore.GetDocumentAsync<Models.Firestore.FirestoreUser>("users", listing.OwnerID);
        string vendorName = vendor?.Name ?? "Unknown Vendor";

        return new()
        {
            ListingId = cartItem.ListingId,
            Quantity = cartItem.Quantity,
            ListingName = listing.Name,
            PriceInCents = listing.Price,
            ThumbnailUrl = listing.Image,
            VendorId = listing.OwnerID,
            VendorName = vendorName
        };
    }
}