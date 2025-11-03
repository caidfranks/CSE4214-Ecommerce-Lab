using GameVault.Shared.Models;

namespace GameVault.Shared.DTOs;

public class InvoiceDTO
{
  public required string Id { get; set; }

  public required InvoiceStatus Status { get; set; }

  public required DateTime OrderDate { get; set; }

  public DateTime? ApprovedDate { get; set; }

  public DateTime? ShippedDate { get; set; }

  public DateTime? CompletedDate { get; set; }

  public DateTime? DeclinedDate { get; set; }

  public DateTime? CancelledDate { get; set; }

  public DateTime? ReturnRequestDate { get; set; }

  public DateTime? ReturnApprovedDate { get; set; }

  // public required string PaymentId { get; set; }

  public required decimal Subtotal { get; set; }

  public required Address ShipTo { get; set; }

  // public required string OrderId { get; set; }

  // public required string VendorId { get; set; }

  public string ReturnMsg { get; set; } = string.Empty;
}

public class FullInvoiceDTO : InvoiceDTO
{
  public required string PyamentId { get; set; }
  public required string OrderId { get; set; }
  public required string VendorId { get; set; }
}

public class InvoiceWithItemsDTO : InvoiceDTO
{
  public required List<InvoiceItemDTO> Items { get; set; }
}

public class InvoiceItemDTO
{
  public required string InvoiceId { get; set; }
  public required string ListingId { get; set; }
  public required int Quantity { get; set; }
  public required int PriceAtOrder { get; set; }
  public required string NameAtOrder { get; set; } = string.Empty;
  public required string DescAtOrder { get; set; } = string.Empty;
}

public class InvoiceUpdateStatusDTO
{
  public required string Id { get; set; }
  public required InvoiceStatus Status { get; set; }
  public string? Message { get; set; }
}