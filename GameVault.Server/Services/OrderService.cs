using GameVault.Server.Models.Firestore;
using GameVault.Shared.DTOs;
using GameVault.Shared.Models;
using Google.Cloud.Firestore;

namespace GameVault.Server.Services;

public class OrderService
{
    private readonly IFirestoreService _firestore;
    private readonly CartService _cartService;
    private readonly SquarePaymentService _paymentService;
    private readonly InvoiceService _invoiceService;
    private readonly TaxService _taxService;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IFirestoreService firestore,
        CartService cartService,
        SquarePaymentService paymentService,
        InvoiceService invoiceService,
        TaxService taxService,
        ILogger<OrderService> logger)
    {
        _firestore = firestore;
        _cartService = cartService;
        _paymentService = paymentService;
        _invoiceService = invoiceService;
        _taxService = taxService;
        _logger = logger;
    }

    public async Task<(bool success, string? orderId, List<string>? invoiceIds, string? errorMessage)>
        CreateOrderFromCartAsync(string customerId, string paymentMethodId, Address shippingAddress)
    {
        try
        {
            var cart = await _cartService.GetCartAsync(customerId);
            if (cart.Items.Count == 0)
            {
                return (false, null, null, "Cart is empty");
            }

            var cartItems = new List<CartItemDTO>();
            foreach (var item in cart.Items)
            {
                var populatedItem = await _cartService.PopulateCartItem(item);
                cartItems.Add(populatedItem);
            }

            // Check all in stock
            foreach (var item in cartItems)
            {
                if (!(await _cartService.IsInStock(item.ListingId, item.Quantity)))
                {
                    return (false, null, null, "One or more products out of stock or quantity too high");
                }
            }

            // Decrease all stock accordingly
            foreach (var item in cartItems)
            {
                await _cartService.DecrementStock(item.ListingId, item.Quantity);
            }

            int subtotalInCents = cartItems.Sum(item => item.PriceInCents * item.Quantity);
            int taxInCents = _taxService.CalculateTax(subtotalInCents, shippingAddress.State);
            int totalInCents = subtotalInCents + taxInCents;

            var order = new Order
            {
                Id = "temp",
                CustomerId = customerId,
                OrderDate = DateTime.UtcNow,
                SubtotalInCents = subtotalInCents,
                TaxInCents = taxInCents,
                TotalInCents = totalInCents,
                ShipTo = shippingAddress
            };

            DocumentReference orderRef = await _firestore.AddDocumentAsync("orders", order);
            order.Id = orderRef.Id;

            _logger.LogInformation("Order created with ID: {orderId}", order.Id);

            var (paymentSuccess, paymentId, paymentError) = await _paymentService.ProcessPaymentAsync(
                totalInCents,
                paymentMethodId,
                customerId,
                order.Id
            );

            if (!paymentSuccess)
            {
                _logger.LogError("Payment failed for order {orderId}: {error}", order.Id, paymentError);

                await _firestore.DeleteDocumentAsync("orders", order.Id);

                return (false, null, null, paymentError ?? "Payment failed");
            }

            _logger.LogInformation("Payment successful for order {orderId}, payment ID: {paymentId}", order.Id, paymentId);

            var itemsByVendor = cartItems.GroupBy(item => item.VendorId);

            var invoiceIds = new List<string>();
            foreach (var vendorGroup in itemsByVendor)
            {
                var vendorId = vendorGroup.Key;
                var vendorItems = vendorGroup.ToList();

                var invoice = await _invoiceService.CreateInvoiceAsync(
                    order.Id,
                    vendorId,
                    vendorItems,
                    shippingAddress,
                    paymentId!
                );

                invoiceIds.Add(invoice.Id);
            }

            await _cartService.ClearCartAsync(customerId);

            _logger.LogInformation("Order {orderId} completed successfully with {invoiceCount} invoices",
                order.Id, invoiceIds.Count);

            return (true, order.Id, invoiceIds, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order from cart");
            return (false, null, null, $"An error occurred: {ex.Message}");
        }
    }

    public async Task<Order?> GetOrderByIdAsync(string orderId)
    {
        try
        {
            var order = await _firestore.GetDocumentAsync<Order>("orders", orderId);
            return order;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order {orderId}", orderId);
            return null;
        }
    }

    public async Task<List<Order>> GetOrdersByCustomerIdAsync(string customerId)
    {
        try
        {
            var orders = await _firestore.QueryDocumentsAsyncWithId<Order>(
                "orders",
                "CustomerId",
                customerId
            );
            return orders.OrderByDescending(o => o.OrderDate).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting orders for customer {customerId}", customerId);
            return new List<Order>();
        }
    }
}