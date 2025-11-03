using Microsoft.AspNetCore.Mvc;
using GameVault.Server.Services;

namespace GameVault.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;
    private readonly ICurrentUserService _currentUser;
    private readonly IFirestoreService _firestore;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        OrderService orderService,
        ICurrentUserService currentUserService,
        IFirestoreService firestore,
        ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _currentUser = currentUserService;
        _firestore = firestore;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyOrders()
    {
        try
        {
            if (!_currentUser.IsAuthenticated || string.IsNullOrEmpty(_currentUser.UserId))
            {
                return Unauthorized(new { error = "User not authenticated" });
            }

            var orders = await _orderService.GetOrdersByCustomerIdAsync(_currentUser.UserId);

            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving orders");
            return StatusCode(500, new { error = "Failed to retrieve orders" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(string id)
    {
        try
        {
            if (!_currentUser.IsAuthenticated || string.IsNullOrEmpty(_currentUser.UserId))
            {
                return Unauthorized(new { error = "User not authenticated" });
            }

            var order = await _orderService.GetOrderByIdAsync(id);

            if (order == null)
            {
                return NotFound(new { error = "Order not found" });
            }

            if (order.CustomerId != _currentUser.UserId)
            {
                return StatusCode(403, new { error = "Access denied" });
            }

            return Ok(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order {orderId}", id);
            return StatusCode(500, new { error = "Failed to retrieve order" });
        }
    }

    [HttpGet("{id}/invoices")]
    public async Task<IActionResult> GetOrderInvoices(string id)
    {
        try
        {
            if (!_currentUser.IsAuthenticated || string.IsNullOrEmpty(_currentUser.UserId))
            {
                return Unauthorized(new { error = "User not authenticated" });
            }

            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound(new { error = "Order not found" });
            }

            if (order.CustomerId != _currentUser.UserId)
            {
                return StatusCode(403, new { error = "Access denied" });
            }

            var invoices = await _firestore.QueryDocumentsAsyncWithId<GameVault.Server.Models.Firestore.Invoice>(
                "invoices",
                "OrderId",
                id
            );

            return Ok(invoices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving invoices for order {orderId}", id);
            return StatusCode(500, new { error = "Failed to retrieve invoices" });
        }
    }

}