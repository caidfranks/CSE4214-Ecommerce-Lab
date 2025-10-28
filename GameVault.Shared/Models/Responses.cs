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

public class AccountListResponse : BaseResponse
{
  public List<AccountDTO>? Accounts { get; set; }
}

public class ListingResponse : BaseResponse
{
  public ListingDTO? Listing { get; set; }
}

public class VendorListingListResponse : BaseResponse
{
  public List<VendorListingDTO>? Listings { get; set; }
}