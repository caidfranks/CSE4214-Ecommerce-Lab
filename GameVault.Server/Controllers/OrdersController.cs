using Microsoft.AspNetCore.Mvc;
using GameVault.Server.Services;
using GameVault.Shared.Models;
using GameVault.Server.Models.Firestore;
using GameVault.Shared.DTOs;

namespace GameVault.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;
    private readonly UserService _userService;
    private readonly IFirestoreService _firestore;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        OrderService orderService,
        UserService userService,
        IFirestoreService firestore,
        ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _userService = userService;
        _firestore = firestore;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ListResponse<OrderDTO>>> GetMyOrders([FromHeader] string? Authorization)
    {
        try
        {
            var user = await _userService.GetUserFromHeader(Authorization);
            if (user is null)
            {
                return Unauthorized();  //new { error = "User not authenticated" });
            }

            if (user.Type != AccountType.Customer)
            {
                return Forbid();
            }

            var orders = await _orderService.GetOrdersByCustomerIdAsync(user.Id);

            List<OrderDTO> orderDTOs = [];

            foreach (Order order in orders)
            {
                orderDTOs.Add(new OrderDTO()
                {
                    Id = order.Id,
                    CustomerId = order.CustomerId,
                    OrderDate = order.OrderDate,
                    SubtotalInCents = order.SubtotalInCents,
                    TaxInCents = order.TaxInCents,
                    TotalInCents = order.TotalInCents
                });
            }

            return Ok(new ListResponse<OrderDTO>()
            {
                Success = true,
                List = orderDTOs
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving orders");
            return StatusCode(500); //, new { error = "Failed to retrieve orders" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrderById(string id, [FromHeader] string? Authorization)
    {
        try
        {
            var user = await _userService.GetUserFromHeader(Authorization);
            if (user is null)
            {
                return Unauthorized(new { error = "User not authenticated" });
            }

            var order = await _orderService.GetOrderByIdAsync(id);

            if (order == null)
            {
                return NotFound(new { error = "Order not found" });
            }

            if (order.CustomerId != user.Id)
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
    public async Task<ActionResult<List<Invoice>>> GetOrderInvoices(string id, [FromHeader] string? Authorization)
    {
        try
        {
            var user = await _userService.GetUserFromHeader(Authorization);
            if (user is null)
            {
                return Unauthorized(new { error = "User not authenticated" });
            }

            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound(new { error = "Order not found" });
            }

            if (order.CustomerId != user.Id)
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