namespace GameVault.Shared.Models;

public enum LogType
{
  ListingStatusChange,
  AccountCreation,
  AccountDeletion,
  VendorAccountRequest,
  VendorApproval,
  AccountBan,
  OrderPlaced,
  InvoiceStatusChange,
  Transaction,
  AccountInfoChange,
  ListingStockChange
}

public enum VendorApproval
{
  Approved = 0,
  Denied = -1
}

public enum AccountInfoChange
{
  Email = 0,
  Password = 1,
  DisplayName = 2
}