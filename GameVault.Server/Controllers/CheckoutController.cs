using Microsoft.AspNetCore.Mvc;
using GameVault.Server.Services;
using GameVault.Shared.DTOs;
using GameVault.Shared.Models;

namespace GameVault.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CheckoutController : ControllerBase
{
    private readonly OrderService _orderService;
    private readonly UserService _userService;
    private readonly ILogger<CheckoutController> _logger;

    public CheckoutController(
        OrderService orderService,
        UserService userService,
        ILogger<CheckoutController> logger)
    {
        _orderService = orderService;
        _userService = userService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> ProcessCheckout(
        [FromBody] CheckoutRequestDTO request,
        [FromHeader] string? Authorization)
    {
        try
        {
            var user = await _userService.GetUserFromHeader(Authorization);
            if (user is null)
            {
                return Unauthorized(new { error = "User not authenticated" });
            }

            if (user.Type != AccountType.Customer)
            {
                return Unauthorized(new { error = "Must be a customer to checkout" });
            }

            _logger.LogInformation("Processing checkout for user {userId}", user.Id);

            if (string.IsNullOrEmpty(request.PaymentMethodId))
            {
                return BadRequest(new { error = "Payment method is required" });
            }

            if (request.ShipTo == null)
            {
                return BadRequest(new { error = "Shipping address is required" });
            }

            var (success, orderId, invoiceIds, errorMessage) = await _orderService.CreateOrderFromCartAsync(
                user.Id,
                request.PaymentMethodId,
                request.ShipTo
            );

            if (!success)
            {
                _logger.LogWarning("Checkout failed for user {userId}: {error}", user.Id, errorMessage);
                return BadRequest(new CheckoutResponseDTO
                {
                    Success = false,
                    ErrorMessage = errorMessage
                });
            }

            _logger.LogInformation("Checkout successful for user {userId}, order {orderId}", user.Id, orderId);

            var order = await _orderService.GetOrderByIdAsync(orderId!);

            return Ok(new CheckoutResponseDTO
            {
                Success = true,
                OrderId = orderId,
                InvoiceIds = invoiceIds ?? new List<string>(),
                TotalAmountInCents = order?.TotalInCents ?? 0,
                ErrorMessage = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during checkout");
            return StatusCode(500, new { error = "An unexpected error occurred during checkout" });
        }
    }

    [HttpPost("estimate-tax")]
    public IActionResult EstimateTax([FromBody] EstimateTaxRequestDTO request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.State))
            {
                return BadRequest(new { error = "State is required" });
            }

            if (request.SubtotalInCents <= 0)
            {
                return BadRequest(new { error = "Subtotal must be greater than 0" });
            }

            var taxService = HttpContext.RequestServices.GetRequiredService<TaxService>();
            var taxInCents = taxService.CalculateTax(request.SubtotalInCents, request.State);

            return Ok(new
            {
                subtotalInCents = request.SubtotalInCents,
                taxInCents = taxInCents,
                totalInCents = request.SubtotalInCents + taxInCents,
                taxRate = taxService.GetTaxRate(request.State)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error estimating tax");
            return StatusCode(500, new { error = "Failed to estimate tax" });
        }
    }
}

public class EstimateTaxRequestDTO
{
    public required int SubtotalInCents { get; set; }
    public required string State { get; set; }
}