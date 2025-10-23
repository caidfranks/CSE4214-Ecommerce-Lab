using System.ComponentModel.DataAnnotations;
using GameVault.Server.Services;
using GameVault.Shared.Models;
using GameVault.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<ActionResult<ShoppingCart>> GetCart()
    {
        try
        {
            var userId = "testUser456";

            _logger.LogInformation("testUser456 accessed cart");

            var cart = await _cartService.GetCartAsync(userId);

            return Ok(cart);
        }
        catch
        {
            _logger.LogError("No cart found");
            return StatusCode(500);
        }
    }
    

    [HttpPost("add")]
    public async Task<ActionResult<CartItem>> AddToCart([FromBody] AddToCartDto addToCartDto)
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

            return Ok(cartItemDetails);
        }
        catch(Exception ex)
        {
            _logger.LogError("Failed to add to cart");
            return StatusCode(500, new { message = ex.Message, details = ex.StackTrace});
        }
    }

    [HttpPut("item/{ListingId}/quantity")]
    public async Task<ActionResult> UpdateCartItemQuantityAsync(
        string listingId,
        [FromBody] int newQuantity)
    {
        try
        {
            var userId = "testUser456";
            _logger.LogInformation("testUser456 updating item");
            var updatedItem = await _cartService.UpdateQuantityAsync(userId, listingId,newQuantity);

            return Ok();
        }
        catch
        {
            _logger.LogError("Failed to update item");
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