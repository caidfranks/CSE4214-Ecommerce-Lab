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
            DescAtOrder = listing?.Description ?? "",
            Rating = RatingChoice.None
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

    public async Task<List<InvoiceDTO>> GetVendorInvoicesByStatusAsync(string vendorId, InvoiceStatus status)
    {
        var invoices = await _firestore.QueryComplexDocumentsAsyncWithId<Invoice>("invoices", [
            new() {
                fieldName = "VendorId",
                value = vendorId
            },
            new() {
                fieldName = "Status",
                value = (int)status
            }
        ]);

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
        return dTOs;
    }

    public async Task<List<InvoiceItem>> GetInvoiceItemsAsync(string invoiceId)
    {
        var allItems = await _firestore.GetCollectionAsync<InvoiceItem>(InvoiceItemsCollection);
        return allItems.Where(item => item.InvoiceId == invoiceId).ToList();
    }

    public async Task<List<InvoiceItem>> GetInvoiceItemsByListingAsync(string listingId)
    {
        return await _firestore.QueryDocumentsAsync<InvoiceItem>(InvoiceItemsCollection, "ListingId", listingId);
    }

    public async Task UpdateInvoiceStatusAsync(
        string invoiceId,
        InvoiceStatus newStatus,
        string? message)
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
            ReturnMsg = message ?? invoice.ReturnMsg
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

    public async Task<InvoiceItemWithId?> GetInvoiceItemWithIdByBothId(string invoiceId, string listingId)
    {
        var results = await _firestore.QueryComplexDocumentsAsyncWithId<InvoiceItemWithId>(InvoiceItemsCollection, [
                new() {
                fieldName = "ListingId",
                value = listingId
            },
            new() {
                fieldName = "InvoiceId",
                value = invoiceId
            }
            ]);

        if (results is null || results.Count < 1)
        {
            return null;
        }

        return results[0];
    }

    public async Task RateInvoiceItem(string id, RatingChoice rating)
    {
        await _firestore.SetDocumentFieldAsync("invoice_items", id, "Rating", (int)rating);
    }

    public async Task CalculateRating(string listingId)
    {
        // Get all Invoice Items with that listingId
        var items = await GetInvoiceItemsByListingAsync(listingId);

        // Get sum of all ratings
        int sum = 0;
        foreach (InvoiceItem item in items)
        {
            sum += (int)item.Rating;
        }

        // Get count of ratings > 0
        int count = 0;
        foreach (InvoiceItem item in items)
        {
            count += (item.Rating != RatingChoice.None) ? 1 : 0;
        }

        // Percent = (sum - count) / count * 100
        int percent = count > 0 ? (sum - count) * 100 / count : -1;
        // -1 means no ratings

        Console.WriteLine($"Updating listing {listingId} rating to {percent} based on {count} reviews");

        // Set to rating field of listing
        // TODO: Make a ListingService on Server to handle this stuff
        await _firestore.SetDocumentFieldAsync("listings", listingId, "Rating", percent);
        await _firestore.SetDocumentFieldAsync("listings", listingId, "NumReviews", count);
    }
}