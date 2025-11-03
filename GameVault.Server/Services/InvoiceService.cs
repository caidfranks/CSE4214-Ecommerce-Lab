using GameVault.Server.Models.Firestore;
using GameVault.Shared.DTOs;
using GameVault.Shared.Models;
using Google.Cloud.Firestore;

namespace GameVault.Server.Services;

public class InvoiceService
{
    private readonly IFirestoreService _firestore;
    private readonly ILogger<InvoiceService> _logger;
    private const string InvoicesCollection = "invoices";
    private const string InvoiceItemsCollection = "invoice_items";
    private const string ListingsCollection = "listings";
    private const string UsersCollection = "users";

    public InvoiceService(IFirestoreService firestore, ILogger<InvoiceService> logger)
    {
        _firestore = firestore;
        _logger = logger;
    }

    public async Task<Invoice> CreateInvoiceAsync(
        string orderId,
        string vendorId,
        List<CartItemDTO> vendorItems,
        Address shipTo,
        string paymentId)
    {
        var subtotal = vendorItems.Sum(item => item.PriceInCents * item.Quantity);

        var firestoreInvoice = new FirestoreInvoice
        {
            OrderId = orderId,
            VendorId = vendorId,
            SubtotalInCents = subtotal,
            Status = InvoiceStatus.Pending,
            OrderDate = DateTime.UtcNow,
            ApprovedDate = null,
            ShippedDate = null,
            CompletedDate = null,
            DeclinedDate = null,
            CancelledDate = null,
            ReturnRequestDate = null,
            ReturnApprovedDate = null,
            ShipTo = shipTo,
            PaymentId = paymentId
        };

        var documentReference = await _firestore.AddDocumentAsync(InvoicesCollection, firestoreInvoice);
        var invoiceId = documentReference.Id;

        foreach (var item in vendorItems)
        {
            await CreateInvoiceItemAsync(invoiceId, item);
        }

        await CreditVendorAsync(vendorId, subtotal);

        return new Invoice
        {
            Id = invoiceId,
            OrderId = orderId,
            VendorId = vendorId,
            SubtotalInCents = subtotal,
            Status = InvoiceStatus.Pending,
            OrderDate = firestoreInvoice.OrderDate,
            ApprovedDate = null,
            ShippedDate = null,
            CompletedDate = null,
            DeclinedDate = null,
            CancelledDate = null,
            ReturnRequestDate = null,
            ReturnApprovedDate = null,
            ShipTo = shipTo,
            PaymentId = paymentId,
            ReturnMsg = string.Empty
        };
    }

    private async Task CreditVendorAsync(string vendorId, int amountInCents)
    {
        try
        {
            var vendor = await _firestore.GetDocumentAsync<User>(UsersCollection, vendorId);
            if (vendor != null && vendor.Type == AccountType.Vendor)
            {
                vendor.BalanceInCents += amountInCents;
                await _firestore.SetDocumentAsync(UsersCollection, vendorId, vendor);
                
                _logger.LogInformation(
                    "Credited vendor {vendorId} with ${amount} (new balance: ${balance})", 
                    vendorId, 
                    amountInCents / 100.0,
                    vendor.BalanceInCents / 100.0
                );
            }
            else
            {
                _logger.LogWarning("Vendor {vendorId} not found or not a vendor account", vendorId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to credit vendor {vendorId}", vendorId);
        }
    }

    private async Task CreateInvoiceItemAsync(string invoiceId, CartItemDTO item)
    {
        var listing = await _firestore.GetDocumentAsync<Listing>(ListingsCollection, item.ListingId);

        var invoiceItem = new InvoiceItem
        {
            InvoiceId = invoiceId,
            ListingId = item.ListingId,
            Quantity = item.Quantity,
            PriceAtOrder = item.PriceInCents,
            NameAtOrder = item.ListingName,
            DescAtOrder = listing?.Description ?? ""
        };

        await _firestore.AddDocumentAsync(InvoiceItemsCollection, invoiceItem);
    }

    public async Task<Invoice?> GetInvoiceByIdAsync(string invoiceId)
    {
        var firestoreInvoice = await _firestore.GetDocumentAsync<FirestoreInvoice>(InvoicesCollection, invoiceId);
        
        if (firestoreInvoice == null)
        {
            return null;
        }

        return new Invoice
        {
            Id = invoiceId,
            OrderId = firestoreInvoice.OrderId,
            VendorId = firestoreInvoice.VendorId,
            SubtotalInCents = firestoreInvoice.SubtotalInCents,
            Status = firestoreInvoice.Status,
            OrderDate = firestoreInvoice.OrderDate,
            ApprovedDate = firestoreInvoice.ApprovedDate,
            ShippedDate = firestoreInvoice.ShippedDate,
            CompletedDate = firestoreInvoice.CompletedDate,
            DeclinedDate = firestoreInvoice.DeclinedDate,
            CancelledDate = firestoreInvoice.CancelledDate,
            ReturnRequestDate = firestoreInvoice.ReturnRequestDate,
            ReturnApprovedDate = firestoreInvoice.ReturnApprovedDate,
            ShipTo = firestoreInvoice.ShipTo,
            PaymentId = firestoreInvoice.PaymentId,
            ReturnMsg = firestoreInvoice.ReturnMsg
        };
    }

    public async Task<List<Invoice>> GetInvoicesByOrderIdAsync(string orderId)
    {
        var allInvoices = await _firestore.GetCollectionAsyncWithId<Invoice>(InvoicesCollection);
        return allInvoices.Where(i => i.OrderId == orderId).ToList();
    }

    public async Task<List<Invoice>> GetInvoicesByVendorIdAsync(string vendorId)
    {
        var allInvoices = await _firestore.GetCollectionAsyncWithId<Invoice>(InvoicesCollection);
        return allInvoices.Where(i => i.VendorId == vendorId).ToList();
    }

    public async Task<List<InvoiceItem>> GetInvoiceItemsAsync(string invoiceId)
    {
        var allItems = await _firestore.GetCollectionAsync<InvoiceItem>(InvoiceItemsCollection);
        return allItems.Where(item => item.InvoiceId == invoiceId).ToList();
    }

    public async Task UpdateInvoiceStatusAsync(
        string invoiceId, 
        InvoiceStatus newStatus)
    {
        var invoice = await GetInvoiceByIdAsync(invoiceId);
        if (invoice == null)
        {
            throw new InvalidOperationException($"Invoice {invoiceId} not found");
        }

        invoice.Status = newStatus;

        switch (newStatus)
        {
            case InvoiceStatus.AwaitingShipment:
                invoice.ApprovedDate = DateTime.UtcNow;
                break;

            case InvoiceStatus.Shipped:
                invoice.ShippedDate = DateTime.UtcNow;
                break;

            case InvoiceStatus.Completed:
                invoice.CompletedDate = DateTime.UtcNow;
                break;

            case InvoiceStatus.Declined:
                invoice.DeclinedDate = DateTime.UtcNow;
                break;

            case InvoiceStatus.Cancelled:
                invoice.CancelledDate = DateTime.UtcNow;
                break;

            case InvoiceStatus.PendingReturn:
                invoice.ReturnRequestDate = DateTime.UtcNow;
                break;

            case InvoiceStatus.AwaitingReturn:
                invoice.ReturnApprovedDate = DateTime.UtcNow;
                break;
        }

        var firestoreInvoice = new FirestoreInvoice
        {
            OrderId = invoice.OrderId,
            VendorId = invoice.VendorId,
            SubtotalInCents = invoice.SubtotalInCents,
            Status = invoice.Status,
            OrderDate = invoice.OrderDate,
            ApprovedDate = invoice.ApprovedDate,
            ShippedDate = invoice.ShippedDate,
            CompletedDate = invoice.CompletedDate,
            DeclinedDate = invoice.DeclinedDate,
            CancelledDate = invoice.CancelledDate,
            ReturnRequestDate = invoice.ReturnRequestDate,
            ReturnApprovedDate = invoice.ReturnApprovedDate,
            ShipTo = invoice.ShipTo,
            PaymentId = invoice.PaymentId,
            ReturnMsg = invoice.ReturnMsg
        };

        await _firestore.SetDocumentAsync(InvoicesCollection, invoiceId, firestoreInvoice);
    }

    public async Task StartReturnAsync(string invoiceId, string returnMessage)
    {
        var invoice = await GetInvoiceByIdAsync(invoiceId);
        if (invoice == null)
        {
            throw new InvalidOperationException($"Invoice {invoiceId} not found");
        }

        invoice.Status = InvoiceStatus.PendingReturn;
        invoice.ReturnMsg = returnMessage;
        invoice.ReturnRequestDate = DateTime.UtcNow;

        var firestoreInvoice = new FirestoreInvoice
        {
            OrderId = invoice.OrderId,
            VendorId = invoice.VendorId,
            SubtotalInCents = invoice.SubtotalInCents,
            Status = invoice.Status,
            OrderDate = invoice.OrderDate,
            ApprovedDate = invoice.ApprovedDate,
            ShippedDate = invoice.ShippedDate,
            CompletedDate = invoice.CompletedDate,
            DeclinedDate = invoice.DeclinedDate,
            CancelledDate = invoice.CancelledDate,
            ReturnRequestDate = invoice.ReturnRequestDate,
            ReturnApprovedDate = invoice.ReturnApprovedDate,
            ShipTo = invoice.ShipTo,
            PaymentId = invoice.PaymentId,
            ReturnMsg = invoice.ReturnMsg
        };

        await _firestore.SetDocumentAsync(InvoicesCollection, invoiceId, firestoreInvoice);
    }
}