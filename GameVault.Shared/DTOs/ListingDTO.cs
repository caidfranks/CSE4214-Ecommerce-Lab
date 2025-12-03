using System.Diagnostics.CodeAnalysis;
using GameVault.Shared.Models;
using Google.Cloud.Firestore;

namespace GameVault.Shared.DTOs;

public class NewListingDTO
{
  public required string Name { get; set; }
  public required int Price { get; set; }
  public string Description { get; set; } = string.Empty;
  public required int Stock { get; set; }
  public required ListingStatus Status { get; set; }
  public required string Category { get; set; }
  public string Image { get; set; } = string.Empty;
}
public class ListingDTO : NewListingDTO
{
  public required string Id { get; set; }
  public required string OwnerID { get; set; }
  public required DateTime LastModified { get; set; }
  public int? Rating { get; set; } // Default to -1
  public int? NumReviews { get; set; } // Default to 0
}

public class FullListingDTO : ListingDTO
{
  public required string VendorName { get; set; }
  public required string CategoryName { get; set; }
}

public class VendorListingDTO : ListingDTO
{
  public required string RemoveMsg { get; set; }
}

public class ListingStockDTO
{
  public required string Id { get; set; }
  public required int AddStock { get; set; }
}