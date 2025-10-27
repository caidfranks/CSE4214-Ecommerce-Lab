using GameVault.Shared.DTOs;

namespace GameVault.Shared.Models;

public class BaseResponse
{
  public bool Success { get; set; }
  public string? Message { get; set; }
}

public class ListingListResponse : BaseResponse
{
  public List<ListingDTO>? Listings { get; set; }
}

public class ListingResponse : BaseResponse
{
  public ListingDTO? Listing { get; set; }
}