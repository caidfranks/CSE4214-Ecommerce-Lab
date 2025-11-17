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
    private readonly UserService _userService;
    private readonly IFirestoreService _firestore;
    private readonly ILogger<InvoiceController> _logger;

    public InvoiceController(
        InvoiceService invoiceService,
        UserService userService,
        IFirestoreService firestore,
        ILogger<InvoiceController> logger)
    {
        _invoiceService = invoiceService;
        _userService = userService;
        _firestore = firestore;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<Invoice>> CreateInvoice(
        [FromBody] CreateInvoiceRequestDTO request,
        [FromHeader] string? Authorization)
    {
        try
        {
            var user = await _userService.GetUserFromHeader(Authorization);
            if (user is null)
            {
                return Unauthorized();
            }

            if (user.Type != AccountType.Customer)
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
    public async Task<ActionResult<Invoice>> GetInvoice(
        string invoiceId,
        [FromHeader] string? Authorization)
    {
        try
        {
            var user = await _userService.GetUserFromHeader(Authorization);
            if (user is null)
            {
                return Unauthorized();
            }

            var invoice = await _invoiceService.GetInvoiceByIdAsync(invoiceId);
            if (invoice == null)
            {
                return NotFound();
            }

            if (user.Type == AccountType.Vendor && invoice.VendorId != user.Id)
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
    public async Task<ActionResult<List<InvoiceDTO>>> GetInvoicesByOrder(
        string orderId,
        [FromHeader] string? Authorization)
    {
        try
        {
            var user = await _userService.GetUserFromHeader(Authorization);
            if (user is null)
            {
                return Unauthorized();
            }

            if (user.Type != AccountType.Customer)
            {
                return Forbid();
            }

            var invoices = await _invoiceService.GetInvoicesByOrderIdAsync(orderId);


            List<InvoiceDTO> dTOs = [];

            foreach (var invoice in invoices)
            {
                InvoiceDTO dTO = new()
                {
                    Id = invoice.Id,
                    Status = invoice.Status,
                    OrderDate = invoice.OrderDate,
                    ApprovedDate = invoice.ApprovedDate,
                    ShippedDate = invoice.ShippedDate,
                    CompletedDate = invoice.CompletedDate,
                    DeclinedDate = invoice.DeclinedDate,
                    CancelledDate = invoice.CancelledDate,
                    ReturnRequestDate = invoice.ReturnRequestDate,
                    ReturnApprovedDate = invoice.ReturnApprovedDate,
                    // PaymentId = invoice.PaymentId,
                    Subtotal = invoice.SubtotalInCents / 100M,
                    ShipTo = invoice.ShipTo,
                    // OrderId = invoice.OrderId,
                    // VendorId = invoice.VendorId,
                    ReturnMsg = invoice.ReturnMsg
                };
                dTOs.Add(dTO);
            }

            return Ok(new ListResponse<InvoiceDTO>()
            {
                Success = true,
                List = dTOs
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invoices by order");
            return StatusCode(500); //, new { error = ex.Message });
        }
    }

    [HttpGet("vendor/{vendorId}")]
    public async Task<ActionResult<List<Invoice>>> GetInvoicesByVendor(
        string vendorId,
        [FromHeader] string? Authorization)
    {
        try
        {
            var user = await _userService.GetUserFromHeader(Authorization);
            if (user is null)
            {
                return Unauthorized();
            }

            if (user.Type == AccountType.Vendor && user.Id != vendorId)
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

    [HttpGet("vendor")]
    public async Task<ActionResult<ListResponse<InvoiceDTO>>> GetVendorInvoicesByStatus([FromQuery] string v, [FromQuery] InvoiceStatus s, [FromHeader] string? Authorization)
    {
        try
        {
            var user = await _userService.GetUserFromHeader(Authorization);
            if (user is null)
            {
                return Unauthorized();
            }

            if (user.Type != AccountType.Vendor || (user.Type == AccountType.Vendor && user.Id != v))
            {
                return Forbid();
            }

            var invoices = await _invoiceService.GetVendorInvoicesByStatusAsync(v, s);

            return Ok(new ListResponse<InvoiceDTO>()
            {
                Success = true,
                List = invoices
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invoices by vendor");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("{invoiceId}/items")]
    public async Task<ActionResult<ListResponse<InvoiceItemDTO>>> GetInvoiceItems(
        string invoiceId,
        [FromHeader] string? Authorization)
    {
        try
        {
            var user = await _userService.GetUserFromHeader(Authorization);
            if (user is null)
            {
                return Unauthorized();
            }

            var invoice = await _invoiceService.GetInvoiceByIdAsync(invoiceId);
            if (invoice == null)
            {
                return NotFound();
            }

            // Also check if owner of order

            if (user.Type == AccountType.Vendor && invoice.VendorId != user.Id)
            {
                return StatusCode(403, new { error = "Access denied" });
            }

            var items = await _invoiceService.GetInvoiceItemsAsync(invoiceId);

            // Convert to DTOs

            List<InvoiceItemDTO> dTOs = [];

            foreach (InvoiceItem item in items)
            {
                InvoiceItemDTO dTO = new()
                {
                    InvoiceId = item.InvoiceId,
                    ListingId = item.ListingId,
                    Quantity = item.Quantity,
                    PriceAtOrder = item.PriceAtOrder,
                    NameAtOrder = item.NameAtOrder,
                    DescAtOrder = item.DescAtOrder,
                    Rating = item.Rating
                };
                dTOs.Add(dTO);
            }

            return Ok(new ListResponse<InvoiceItemDTO> { Success = true, List = dTOs });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invoice items");
            return StatusCode(500, new ListResponse<InvoiceItemDTO> { Success = false, Message = ex.Message });
        }
    }

    [HttpPut("{invoiceId}/status")]
    public async Task<ActionResult> UpdateInvoiceStatus(
        string invoiceId,
        [FromBody] UpdateStatusRequestDTO request,
        [FromHeader] string? Authorization)
    {
        try
        {
            var user = await _userService.GetUserFromHeader(Authorization);
            if (user is null)
            {
                return Unauthorized();
            }

            if (user.Type != AccountType.Vendor)
            {
                return Unauthorized();
            }

            var invoice = await _invoiceService.GetInvoiceByIdAsync(invoiceId);
            if (invoice == null)
            {
                return NotFound();
            }

            if (invoice.VendorId != user.Id)
            {
                return StatusCode(403, new { error = "Access denied" });
            }

            await _invoiceService.UpdateInvoiceStatusAsync(invoiceId, request.Status, null);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating invoice status");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("update-status")]
    public async Task<ActionResult<BaseResponse>> UpdateInvoiceStatusWithMessage([FromBody] InvoiceUpdateStatusDTO dTO, [FromHeader] string? Authorization)
    {
        var user = await _userService.GetUserFromHeader(Authorization);
        if (user is null)
        {
            return Unauthorized();
        }

        var invoice = await _invoiceService.GetInvoiceByIdAsync(dTO.Id);
        if (invoice == null)
        {
            return NotFound();
        }

        // 2 Success Cases:
        // 1) Is vendor and owner of invoice
        if (user.Type == AccountType.Vendor && invoice.VendorId == user.Id)
        {
            // Can change most statuses

        }
        // 2) Is customer and recipient of invoice
        else if (user.Type == AccountType.Customer)
        {
            // Look up order
            // Confirm customerId == user.Id
            // Can set status to pendingReturn from completed
            // Order cancellation done from OrderController

        }
        else
        {
            return Forbid();
        }

        await _invoiceService.UpdateInvoiceStatusAsync(dTO.Id, dTO.Status, dTO.Message);

        return Ok();
    }

    [HttpPost("rate")]
    public async Task<ActionResult<BaseResponse>> RateInvoiceItem([FromBody] RatingDTO dTO, [FromHeader] string? Authorization)
    {
        var user = await _userService.GetUserFromHeader(Authorization);
        if (user is null)
        {
            return Unauthorized();
        }

        // Customer
        if (user.Type != AccountType.Customer)
        {
            return Forbid();
        }

        // TODO: Look up invoice, then order, and confirm owner

        // Query "invoice_items" for provided listingId and invoiceId

        var invoiceItem = await _invoiceService.GetInvoiceItemWithIdByBothId(dTO.InvoiceId, dTO.ListingId);

        if (invoiceItem is null)
        {
            return NotFound(new BaseResponse
            {
                Success = false,
                Message = "Invoice item not found"
            });
        }

        // Use Id only
        await _invoiceService.RateInvoiceItem(invoiceItem.Id, dTO.Rating);

        // Run in background since not relevant to this customer right now
        // _ = // Discards result without having to await the async call
        _ = _invoiceService.CalculateRating(dTO.ListingId);

        return Ok(new BaseResponse
        {
            Success = true,
            Message = "Rating successfully updated"
        });
    }
}