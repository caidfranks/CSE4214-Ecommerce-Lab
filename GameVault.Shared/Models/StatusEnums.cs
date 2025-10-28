namespace GameVault.Shared.Models;

public enum InvoiceStatus
{
  Pending = 0,
  AwaitingShipment = 1,
  Shipped = 2,
  Completed = 3,
  Declined = 4,
  Cancelled = 5,
  PendingReturn = -1, // User prompted
  AwaitingReturn = -2 // Vendor accepted
}

public enum AccountType
{
  Admin = 0,
  Vendor = 1,
  Customer = 2
}

public enum ListingStatus
{
  Removed = -1,
  Inactive = 0,
  Pending = 1,
  Published = 2
}

public enum AccountStatus { 
    Denied = -1,
    PendingVendor = 0,
    ActiveVendor = 1,
    Customer = 2,
    Banned = 3
}
