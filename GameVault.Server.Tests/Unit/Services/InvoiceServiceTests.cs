using GameVault.Server.Models.Firestore;
using GameVault.Server.Services;
using GameVault.Shared.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameVault.Server.Tests.Unit.Services;

public class InvoiceServiceTests
{
    private readonly Mock<IFirestoreService> _mockFirestore;
    private readonly Mock<ILogger<InvoiceService>> _mockLogger;
    private readonly InvoiceService _invoiceService;

    public InvoiceServiceTests()
    {
        _mockFirestore = new Mock<IFirestoreService>();
        _mockLogger = new Mock<ILogger<InvoiceService>>();
        _invoiceService = new InvoiceService(_mockFirestore.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetInvoiceByIdAsync_WhenInvoiceExists_ReturnsInvoice()
    {
        var invoiceId = "invoice123";
        var firestoreInvoice = new FirestoreInvoice
        {
            OrderId = "order1",
            VendorId = "vendor1",
            SubtotalInCents = 5000,
            Status = InvoiceStatus.Pending,
            OrderDate = DateTime.UtcNow,
            ShipTo = new Address { Street = "123 Main St", City = "Austin", State = "TX", PostalCode = "78701" },
            PaymentId = "pay_123"
        };

        _mockFirestore
            .Setup(x => x.GetDocumentAsync<FirestoreInvoice>("invoices", invoiceId))
            .ReturnsAsync(firestoreInvoice);

        var result = await _invoiceService.GetInvoiceByIdAsync(invoiceId);

        Assert.NotNull(result);
        Assert.Equal(invoiceId, result.Id);
        Assert.Equal("order1", result.OrderId);
        Assert.Equal(InvoiceStatus.Pending, result.Status);
    }

    [Fact]
    public async Task GetInvoiceByIdAsync_WhenInvoiceDoesNotExist_ReturnsNull()
    {
        var invoiceId = "nonexistent";

        _mockFirestore
            .Setup(x => x.GetDocumentAsync<FirestoreInvoice>("invoices", invoiceId))
            .ReturnsAsync((FirestoreInvoice?)null);

        var result = await _invoiceService.GetInvoiceByIdAsync(invoiceId);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateInvoiceStatusAsync_WhenInvoiceNotFound_ThrowsException()
    {
        var invoiceId = "nonexistent";

        _mockFirestore
            .Setup(x => x.GetDocumentAsync<FirestoreInvoice>("invoices", invoiceId))
            .ReturnsAsync((FirestoreInvoice?)null);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _invoiceService.UpdateInvoiceStatusAsync(invoiceId, InvoiceStatus.AwaitingShipment, null));
    }

    [Fact]
    public async Task UpdateInvoiceStatusAsync_SetsShippedDate_WhenStatusIsShipped()
    {
        var invoiceId = "invoice1";
        var firestoreInvoice = new FirestoreInvoice
        {
            OrderId = "order1",
            VendorId = "vendor1",
            SubtotalInCents = 5000,
            Status = InvoiceStatus.AwaitingShipment,
            OrderDate = DateTime.UtcNow.AddDays(-1),
            ApprovedDate = DateTime.UtcNow,
            ShipTo = new Address { State = "TX" },
            PaymentId = "pay_1"
        };

        _mockFirestore
            .Setup(x => x.GetDocumentAsync<FirestoreInvoice>("invoices", invoiceId))
            .ReturnsAsync(firestoreInvoice);

        await _invoiceService.UpdateInvoiceStatusAsync(invoiceId, InvoiceStatus.Shipped, null);

        _mockFirestore.Verify(
            x => x.SetDocumentAsync("invoices", invoiceId, It.Is<FirestoreInvoice>(inv =>
                inv.Status == InvoiceStatus.Shipped &&
                inv.ShippedDate != null)),
            Times.Once);
    }

    [Fact]
    public async Task StartReturnAsync_SetsReturnFieldsCorrectly()
    {
        var invoiceId = "invoice1";
        var returnMessage = "Item arrived damaged";
        var firestoreInvoice = new FirestoreInvoice
        {
            OrderId = "order1",
            VendorId = "vendor1",
            SubtotalInCents = 5000,
            Status = InvoiceStatus.Completed,
            OrderDate = DateTime.UtcNow.AddDays(-5),
            CompletedDate = DateTime.UtcNow.AddDays(-1),
            ShipTo = new Address { State = "TX" },
            PaymentId = "pay_1"
        };

        _mockFirestore
            .Setup(x => x.GetDocumentAsync<FirestoreInvoice>("invoices", invoiceId))
            .ReturnsAsync(firestoreInvoice);

        await _invoiceService.StartReturnAsync(invoiceId, returnMessage);

        _mockFirestore.Verify(
            x => x.SetDocumentAsync("invoices", invoiceId, It.Is<FirestoreInvoice>(inv =>
                inv.Status == InvoiceStatus.PendingReturn &&
                inv.ReturnMsg == returnMessage &&
                inv.ReturnRequestDate != null)),
            Times.Once);
    }
}
