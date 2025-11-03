using GameVault.Server.Services;
using GameVault.Server.Models;
using GameVault.Server.Models.Firestore;
using GameVault.Shared.DTOs;
using GameVault.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace GameVault.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoiceController : ControllerBase
{
    private readonly InvoiceService _invoiceService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFirestoreService _firestore;
    private readonly ILogger<InvoiceController> _logger;

    public InvoiceController(
        InvoiceService invoiceService,
        ICurrentUserService currentUserService,
        IFirestoreService firestore,
        ILogger<InvoiceController> logger)
    {
        _invoiceService = invoiceService;
        _currentUserService = currentUserService;
        _firestore = firestore;
        _logger = logger;
    }
    
    [HttpPost]
    public async Task<ActionResult<Invoice>> CreateInvoice([FromBody] CreateInvoiceRequest request)
    {
        try
        {
            if (!_currentUserService.IsAuthenticated || string.IsNullOrEmpty(_currentUserService.UserId))
            {
                return Unauthorized();
            }

            var invoice = await _invoiceService.CreateInvoiceAsync(
                request.OrderId,
                request.VendorId,
                request.VendorItems,
                request.ShipTo,
                request.PaymentId
            );

            return Ok(invoice);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating invoice");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("{invoiceId}")]
    public async Task<ActionResult<Invoice>> GetInvoice(string invoiceId)
    {
        try
        {
            if (!_currentUserService.IsAuthenticated || string.IsNullOrEmpty(_currentUserService.UserId))
            {
                return Unauthorized();
            }

            var invoice = await _invoiceService.GetInvoiceByIdAsync(invoiceId);
            if (invoice == null)
            {
                return NotFound();
            }

            var user = await _firestore.GetDocumentAsync<FirestoreUser>("users", _currentUserService.UserId);
            if (user == null)
            {
                return Unauthorized();
            }

            if (user.Type == AccountType.Vendor && invoice.VendorId != _currentUserService.UserId)
            {
                return StatusCode(403, new { error = "Access denied" });
            }

            return Ok(invoice);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invoice");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("order/{orderId}")]
    public async Task<ActionResult<List<Invoice>>> GetInvoicesByOrder(string orderId)
    {
        try
        {
            if (!_currentUserService.IsAuthenticated || string.IsNullOrEmpty(_currentUserService.UserId))
            {
                return Unauthorized();
            }

            var invoices = await _invoiceService.GetInvoicesByOrderIdAsync(orderId);
            return Ok(invoices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invoices by order");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("vendor/{vendorId}")]
    public async Task<ActionResult<List<Invoice>>> GetInvoicesByVendor(string vendorId)
    {
        try
        {
            if (!_currentUserService.IsAuthenticated || string.IsNullOrEmpty(_currentUserService.UserId))
            {
                return Unauthorized();
            }

            var user = await _firestore.GetDocumentAsync<FirestoreUser>("users", _currentUserService.UserId);
            if (user == null)
            {
                return Unauthorized();
            }

            if (user.Type == AccountType.Vendor && _currentUserService.UserId != vendorId)
            {
                return StatusCode(403, new { error = "Access denied" });
            }

            var invoices = await _invoiceService.GetInvoicesByVendorIdAsync(vendorId);
            return Ok(invoices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invoices by vendor");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("{invoiceId}/items")]
    public async Task<ActionResult<List<InvoiceItem>>> GetInvoiceItems(string invoiceId)
    {
        try
        {
            if (!_currentUserService.IsAuthenticated || string.IsNullOrEmpty(_currentUserService.UserId))
            {
                return Unauthorized();
            }

            var invoice = await _invoiceService.GetInvoiceByIdAsync(invoiceId);
            if (invoice == null)
            {
                return NotFound();
            }

            var user = await _firestore.GetDocumentAsync<FirestoreUser>("users", _currentUserService.UserId);
            if (user == null)
            {
                return Unauthorized();
            }

            if (user.Type == AccountType.Vendor && invoice.VendorId != _currentUserService.UserId)
            {
                return StatusCode(403, new { error = "Access denied" });
            }

            var items = await _invoiceService.GetInvoiceItemsAsync(invoiceId);
            return Ok(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invoice items");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPut("{invoiceId}/status")]
    public async Task<ActionResult> UpdateInvoiceStatus(
        string invoiceId,
        [FromBody] UpdateStatusRequest request)
    {
        try
        {
            if (!_currentUserService.IsAuthenticated || string.IsNullOrEmpty(_currentUserService.UserId))
            {
                return Unauthorized();
            }

            var user = await _firestore.GetDocumentAsync<FirestoreUser>("users", _currentUserService.UserId);
            if (user == null || user.Type != AccountType.Vendor)
            {
                return Unauthorized();
            }

            var invoice = await _invoiceService.GetInvoiceByIdAsync(invoiceId);
            if (invoice == null)
            {
                return NotFound();
            }

            if (invoice.VendorId != _currentUserService.UserId)
            {
                return StatusCode(403, new { error = "Access denied" });
            }

            await _invoiceService.UpdateInvoiceStatusAsync(invoiceId, request.Status);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating invoice status");
            return StatusCode(500, new { error = ex.Message });
        }
    }
}

public class CreateInvoiceRequest
{
    public required string OrderId { get; set; }
    public required string VendorId { get; set; }
    public required List<CartItemDTO> VendorItems { get; set; }
    public required Address ShipTo { get; set; }
    public required string PaymentId { get; set; }
}

public class UpdateStatusRequest
{
    public required InvoiceStatus Status { get; set; }
}