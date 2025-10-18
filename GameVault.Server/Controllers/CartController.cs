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

            var cart = _cartService.GetCartAsync(userId);

            return Ok(cart);
        }
        catch
        {
            _logger.LogError("No cart found");
            return StatusCode(500);
        }
    } 
}