using GameVault.Server.Services;
using GameVault.Shared.Models;
using GameVault.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore.V1;
using Microsoft.AspNetCore.Authorization;
using GameVault.Server.Filters;
using GameVault.Server.Models;

namespace GameVault.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly CartService _cartService;
    private readonly ILogger<CartController> _logger;
    private readonly UserService _userService;

    public CartController(CartService cartService, ILogger<CartController> logger, UserService userService)
    {
        _cartService = cartService;
        _logger = logger;
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<CartDTO>> GetCart([FromHeader] string? Authorization)
    {
        try
        {
            var user = await _userService.GetUserFromHeader(Authorization);

            if (user is null)
            {
                return Forbid();
            }
            else if (user.Role != nameof(UserRole.Customer))
            {
                return Unauthorized();
            }

            _logger.LogInformation("{UserId} accessed cart", user.UserId);

            var dbCart = await _cartService.GetCartAsync(user.UserId);

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
    public async Task<ActionResult<CartItemDTO>> AddToCart([FromBody] NewCartItemDTO addToCartDto, [FromHeader] string? Authorization)
    {
        try
        {
            var user = await _userService.GetUserFromHeader(Authorization);

            if (user is null)
            {
                return Forbid();
            }
            else if (user.Role != nameof(UserRole.Customer))
            {
                return Unauthorized();
            }

            _logger.LogInformation("{UserId} adding to cart", user.UserId);

            var cartItemDetails = await _cartService.AddToCartAsync(
                user.UserId,
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
        [FromBody] UpdateCartItemQuantityRequest request, [FromHeader] string Authorization)
    {
        try
        {
            var user = await _userService.GetUserFromHeader(Authorization);

            if (user is null)
            {
                return Forbid();
            }
            else if (user.Role != nameof(UserRole.Customer))
            {
                return Unauthorized();
            }

            _logger.LogInformation("{UserId} updating item", user.UserId);

            var updatedItem = await _cartService.UpdateQuantityAsync(user.UserId, listingId, request.Quantity);

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
    public async Task<ActionResult> RemoveFromCartAsync([FromRoute] string listingId, [FromHeader] string Authorization)
    {
        try
        {
            var user = await _userService.GetUserFromHeader(Authorization);

            if (user is null)
            {
                return Forbid();
            }
            else if (user.Role != nameof(UserRole.Customer))
            {
                return Unauthorized();
            }

            _logger.LogInformation("{UserId} removing item", user.UserId);

            await _cartService.RemoveFromCartAsync(user.UserId, listingId);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove item");
            return StatusCode(500);
        }
    }

    [HttpDelete]
    public async Task<ActionResult> ClearCartAsync([FromHeader] string Authorization)
    {
        try
        {
            var user = await _userService.GetUserFromHeader(Authorization);

            if (user is null)
            {
                return Forbid();
            }
            else if (user.Role != nameof(UserRole.Customer))
            {
                return Unauthorized();
            }

            _logger.LogInformation("{UserId} clearing cart", user.UserId);

            await _cartService.ClearCartAsync(user.UserId);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear cart");

            return StatusCode(500);
        }
    }
}