using GameVault.Shared.DTOs;
using GameVault.Shared.Models;
using Google.Cloud.Firestore.V1;
using Grpc.Net.Client.Balancer;
using System;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace GameVault.Client.Services;

public class LogService
{
    private readonly HttpClient _httpClient;

    public LogService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LogListResponse> GetAllLogsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/log");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error fetching products: {response.StatusCode}");
                return new LogListResponse
                {
                    Success = false,
                    Message = "Error fetching logs"
                };
            }

            var result = await response.Content.ReadFromJsonAsync<LogListResponse>();

            return result ?? new LogListResponse
            {
                Success = false,
                Message = "Unknown error"
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching products: {ex.Message}");
            return new LogListResponse
            {
                Success = false,
                Message = "Unknown error"
            }; ;
        }
    }

    public async Task CreateLogAsync(LogType type, string objectId, string summary, int status = 0, string? details = null)
    {
        LogDTO log = new()
        {
            Id = objectId,
            Summary = summary,
            Type = type,
            ObjectId = objectId,
            Status = status,
            Timestamp = DateTime.UtcNow,
            Details = details ?? summary
        };

        await _httpClient.PostAsJsonAsync("api/log", log);
     
    }

    public Task LogListingStatusChangeAsync(string listingId, string oldStatus, string newStatus, string? details = null)
    {
        return CreateLogAsync(
            LogType.ListingStatusChange,
            listingId,
            $"Listing status changed from {oldStatus} to {newStatus}",
            0,
            details ?? $"Status transition: {oldStatus} ? {newStatus}"
        );
    }

    public Task LogAccountCreationAsync(string accountId, string email, string? details = null)
    {
        return CreateLogAsync(
            LogType.AccountCreation,
            accountId,
            $"New account created: {email}",
            0,
            details ?? $"Account created for email: {email}"
        );
    }

    public Task LogAccountDeletionAsync(string accountId, string email, string reason, string? details = null)
    {
        return CreateLogAsync(
            LogType.AccountDeletion,
            accountId,
            $"Account deleted: {email}",
            0,
            details ?? $"Account deleted. Reason: {reason}"
        );
    }

    public Task LogVendorAccountRequestAsync(string accountId, string email, string? details = null)
    {
        return CreateLogAsync(
            LogType.VendorAccountRequest,
            accountId,
            $"Vendor account requested by {email}",
            0,
            details ?? "Pending vendor approval"
        );
    }

    public Task LogVendorApprovalAsync(string accountId, string email, VendorApproval approvalStatus)//, string? details = null)
    {
        var statusText = approvalStatus == VendorApproval.Approved ? "approved" : "denied";
        return CreateLogAsync(
            LogType.VendorApproval,
            accountId,
            $"Vendor account {statusText} for {email}"
            //(int)approvalStatus,
            //details ?? $"Vendor account request was {statusText}"
        );
    }

    public Task LogAccountBanAsync(string accountId, string email, string reason)
    {
        return CreateLogAsync(
            LogType.AccountBan,
            accountId,
            $"Reason: {reason} for {email}"
        );
    }

    public Task LogOrderPlacedAsync(string orderId, string customerId, decimal totalAmount, int itemCount, string? details = null)
    {
        return CreateLogAsync(
            LogType.OrderPlaced,
            orderId,
            $"Order placed: {itemCount} item(s) totaling ${totalAmount / 100}",
            0,
            details ?? $"Customer: {customerId}, Total: ${totalAmount / 100}, Items: {itemCount}"
        );
    }

    public Task LogInvoiceStatusChangeAsync(string invoiceId, string oldStatus, string newStatus, string? details = null)
    {
        return CreateLogAsync(
            LogType.InvoiceStatusChange,
            invoiceId,
            $"Invoice status changed from {oldStatus} to {newStatus}",
            0,
            details ?? $"Invoice status transition: {oldStatus} ? {newStatus}"
        );
    }

    public Task LogTransactionAsync(string transactionId, string type, decimal amount)//, int status, string? details = null)
    {
        return CreateLogAsync(
            LogType.Transaction,
            transactionId,
            $"{type} transaction: ${amount / 2}",
            0
            //details ?? $"Transaction type: {type}, Amount: ${amount:F2}"
        );
    }

    public Task LogAccountInfoChangeAsync(string accountId, AccountInfoChange changeType, string oldValue, string newValue, string? details = null)
    {
        var changeDescription = changeType switch
        {
            AccountInfoChange.Email => "Email address",
            AccountInfoChange.Password => "Password",
            AccountInfoChange.DisplayName => "Display name",
            _ => "Account information"
        };

        var summary = changeType == AccountInfoChange.Password
            ? $"{changeDescription} changed"
            : $"{changeDescription} changed from {oldValue} to {newValue}";

        return CreateLogAsync(
            LogType.AccountInfoChange,
            accountId,
            summary,
            (int)changeType,
            details ?? summary
        );
    }

    public Task LogListingStockChangeAsync(string listingId, int oldStock, int newStock, string reason, string? details = null)
    {
        var difference = newStock - oldStock;
        var changeType = difference > 0 ? "increased" : "decreased";

        return CreateLogAsync(
            LogType.ListingStockChange,
            listingId,
            $"Stock {changeType} by {Math.Abs(difference)} units (from {oldStock} to {newStock})",
            difference,
            details ?? $"Stock change: {oldStock} ? {newStock}. Reason: {reason}"
        );
    }
}