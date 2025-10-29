using System.ComponentModel.DataAnnotations;
using GameVault.Server.Services;
using GameVault.Shared.Models;
using GameVault.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore.V1;
//using Microsoft.AspNetCore.Authorization;

namespace GameVault.Server.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly CartService _cartService;
    private readonly ILogger<CartController> _logger;

    public CartController(CartService cartService, ILogger<CartController> logger)
    {
        _cartService = cartService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<CartDTO>> GetCart()
    {
        try
        {
            var userId = "testUser456";

            _logger.LogInformation("testUser456 accessed cart");

            var dbCart = await _cartService.GetCartAsync(userId);

            List<CartItemDTO> items = [];
            foreach (Models.Firestore.CartItem item in dbCart.Items)
            {
                items.Add(await _cartService.PopulateCartItem(item));
            }

            CartDTO cart = new()
            {
                UserId = dbCart.OwnerId,
                Items = items
            };

            return Ok(cart);
        }
        catch
        {
            _logger.LogError("No cart found");
            return StatusCode(500);
        }
    }


    [HttpPost("item")]
    public async Task<ActionResult<CartItemDTO>> AddToCart([FromBody] NewCartItemDTO addToCartDto)
    {
        try
        {
            var userId = "testUser456";

            _logger.LogInformation("testUser456 adding to cart");

            var cartItemDetails = await _cartService.AddToCartAsync(
                userId,
                addToCartDto.ListingId,
                addToCartDto.Quantity
            );

            CartItemDTO fullItem = await _cartService.PopulateCartItem(cartItemDetails);

            return Ok(fullItem);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to add to cart");
            return StatusCode(500, new { message = ex.Message, details = ex.StackTrace });
        }
    }

    [HttpPut("item/{listingId}/quantity")]
    public async Task<ActionResult> UpdateCartItemQuantityAsync(
        string listingId,
        [FromBody] UpdateCartItemQuantityRequest request)
    {
        try
        {
            var userId = "testUser456";
            _logger.LogInformation("testUser456 updating item");
            var updatedItem = await _cartService.UpdateQuantityAsync(userId, listingId, request.Quantity);

            CartItemDTO fullItem = await _cartService.PopulateCartItem(updatedItem);

            return Ok(fullItem);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to update item");
            Console.WriteLine(ex.Message);
            return StatusCode(500);

        }
    }

    [HttpDelete("item/{ListingId}")]
    public async Task<ActionResult> RemoveFromCartAsync([FromRoute] string listingId)
    {
        try
        {
            var userId = "testUser456";
            _logger.LogInformation("testUser456 removing item");

            await _cartService.RemoveFromCartAsync(userId, listingId);

            return Ok();
        }
        catch
        {
            _logger.LogError("Failed to remove item");
            return StatusCode(500);
        }
    }

    [HttpDelete]
    public async Task<ActionResult> ClearCartAsync()
    {
        try
        {
            var userId = "testUser456";
            _logger.LogInformation("testUser456 clearing cart");

            await _cartService.ClearCartAsync(userId);

            return Ok();
        }
        catch
        {
            _logger.LogError("Failed to clear cart");
            return StatusCode(500);
        }
    }
}