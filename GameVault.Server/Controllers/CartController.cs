using GameVault.Server.Services;
using GameVault.Shared.Models;
using GameVault.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore.V1;
using Microsoft.AspNetCore.Authorization;
using GameVault.Server.Filters;

namespace GameVault.Server.Controllers;

[Authorize]
[RequireAuthUser]
[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly CartService _cartService;
    private readonly ILogger<CartController> _logger;
    private readonly ICurrentUserService _currentUser;

    public CartController(CartService cartService, ILogger<CartController> logger, ICurrentUserService currentUser)
    {
        _cartService = cartService;
        _logger = logger;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<ActionResult<CartDTO>> GetCart()
    {
        try
        {
            _logger.LogInformation("{UserId} accessed cart", _currentUser.UserId);

            _logger.LogInformation("testUser456 accessed cart");

            var dbCart = await _cartService.GetCartAsync(_currentUser.UserId);

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "No cart found");
            return StatusCode(500);
        }
    }


    [HttpPost("item")]
    public async Task<ActionResult<CartItemDTO>> AddToCart([FromBody] NewCartItemDTO addToCartDto)
    {
        try
        {
            _logger.LogInformation("{UserId} adding to cart", _currentUser.UserId);

            var cartItemDetails = await _cartService.AddToCartAsync(
                _currentUser.UserId,
                addToCartDto.ListingId,
                addToCartDto.Quantity
            );

            CartItemDTO fullItem = await _cartService.PopulateCartItem(cartItemDetails);

            return Ok(fullItem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add to cart");

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
            _logger.LogInformation("{UserId} updating item", _currentUser.UserId);

            var updatedItem = await _cartService.UpdateQuantityAsync(_currentUser.UserId, listingId, request.Quantity);

            CartItemDTO fullItem = await _cartService.PopulateCartItem(updatedItem);

            return Ok(fullItem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update item");
            return StatusCode(500);

        }
    }

    [HttpDelete("item/{ListingId}")]
    public async Task<ActionResult> RemoveFromCartAsync([FromRoute] string listingId)
    {
        try
        {
            _logger.LogInformation("{UserId} removing item", _currentUser.UserId);

            await _cartService.RemoveFromCartAsync(_currentUser.UserId, listingId);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove item");
            return StatusCode(500);
        }
    }

    [HttpDelete]
    public async Task<ActionResult> ClearCartAsync()
    {
        try
        {
            _logger.LogInformation("{UserId} clearing cart", _currentUser.UserId);

            await _cartService.ClearCartAsync(_currentUser.UserId);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear cart");

            return StatusCode(500);
        }
    }
}