using System.ComponentModel.DataAnnotations;
using GameVault.Server.Services;
using GameVault.Shared.Models;
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
                addToCartDto.ListingName,
                addToCartDto.ThumbnailUrl,
                addToCartDto.PriceInCents,
                addToCartDto.Quantity,
                addToCartDto.VendorId,
                addToCartDto.VendorName
            );

            return Ok(cartItemDetails);
        }
        catch(Exception ex)
        {
            _logger.LogError("Failed to add to cart");
            return StatusCode(500, new { message = ex.Message, details = ex.StackTrace});
        }
    }

    [HttpPut("item/{cartItemId}/quantity")]
    public async Task<ActionResult> UpdateCartItemQuantityAsync([FromBody] UpdateCartItemQuantityDto updateDetails,
    string cartItemId)
    {
        try
        {
            var userId = "testUser456";
            _logger.LogInformation("testUser456 updating item");

            var updatedItem = await _cartService.UpdateQuantityAsync(
                userId,
                cartItemId,
                updateDetails.newQuantity);

            return Ok(updatedItem);
        }
        catch
        {
            _logger.LogError("Failed to update item");
            return StatusCode(500);

        }
    }

    [HttpDelete("item/{cartItemId}")]
    public async Task<ActionResult> RemoveFromCartAsync([FromRoute] string cartItemId)
    {
        try
        {
            var userId = "testUser456";
            _logger.LogInformation("testUser456 removing item");

            await _cartService.RemoveFromCartAsync(userId, cartItemId);

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

public class AddToCartDto
{
    public string ListingId { get; set; }
    public string ListingName  { get; set; }
    public string ThumbnailUrl { get; set; }
    public int PriceInCents { get; set; }
    public int Quantity { get; set; }
    public string VendorId { get; set; }
    public string VendorName { get; set; }
}

public class UpdateCartItemQuantityDto
{
    [Required]
    public int newQuantity { get; set; }
}