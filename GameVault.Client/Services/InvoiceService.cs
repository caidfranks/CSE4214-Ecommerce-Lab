using System.Net.Http.Json;
using GameVault.Shared.DTOs;
using GameVault.Shared.Models;
using Google.Apis.Auth.OAuth2;

namespace GameVault.Client.Services;

public class InvoiceService
{
  private readonly HttpClient _httpClient;

  public InvoiceService(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public async Task<ListResponse<InvoiceDTO>> GetVendorInvoicesByStatus(string userId, InvoiceStatus status)
  {
    System.Net.HttpStatusCode? statusCode = null;
    try
    {
      var response = await _httpClient.GetAsync($"api/invoice/vendor?v={Uri.EscapeDataString(userId)}&s={status}");
      statusCode = response.StatusCode;
      var result = await response.Content.ReadFromJsonAsync<ListResponse<InvoiceDTO>>();
      return result ?? new ListResponse<InvoiceDTO> { Success = false, Message = "Unknown error" };
    }
    catch (Exception ex)
    {
      if (statusCode is not null)
      {
        Console.Write($"Get vendor invoices failed with status: {statusCode}");
        // TODO: Could narrow down errors from status code
      }
      else
      {
        Console.Write("Unexpected error getting vendor invoices:");
        Console.Write(ex);
      }
      return new ListResponse<InvoiceDTO> { Success = false, Message = "Unknown error" };
    }
  }

  public async Task<BaseResponse> SetOrderStatus(string id, InvoiceStatus newStatus)
  {
    try
    {
      InvoiceUpdateStatusDTO msg = new()
      {
        Id = id,
        Status = newStatus,
      };
      var response = await _httpClient.PostAsJsonAsync("api/invoice/update-status", msg);

      var result = await response.Content.ReadFromJsonAsync<BaseResponse>();

      return result ?? new BaseResponse { Success = false, Message = "Unknown error" };
    }
    catch (Exception ex)
    {
      Console.WriteLine("Error setting order status:");
      Console.WriteLine(ex);
      return new BaseResponse { Success = false, Message = "Unknown error" };
    }
  }

  public async Task<BaseResponse> SetOrderStatusWithMessage(string id, InvoiceStatus newStatus, string message)
  {
    try
    {
      InvoiceUpdateStatusDTO msg = new()
      {
        Id = id,
        Status = newStatus,
        Message = message
      };
      var response = await _httpClient.PostAsJsonAsync("api/invoice/update-status", msg);

      var result = await response.Content.ReadFromJsonAsync<BaseResponse>();

      return result ?? new BaseResponse { Success = false, Message = "Unknown error" };
    }
    catch (Exception ex)
    {
      Console.WriteLine("Error setting order status with message:");
      Console.WriteLine(ex);
      return new BaseResponse { Success = false, Message = "Unknown error" };
    }
  }

  public async Task<ListResponse<InvoiceDTO>> GetInvoicesByOrder(string orderId)
  {
    System.Net.HttpStatusCode? statusCode = null;
    try
    {
      var response = await _httpClient.GetAsync($"api/invoice/order/{Uri.EscapeDataString(orderId)}");
      statusCode = response.StatusCode;
      var result = await response.Content.ReadFromJsonAsync<ListResponse<InvoiceDTO>>();
      return result ?? new ListResponse<InvoiceDTO> { Success = false, Message = "Unknown error" };
    }
    catch (Exception ex)
    {
      if (statusCode is not null)
      {
        Console.Write($"Get invoices for order failed with status: {statusCode}");
        // TODO: Could narrow down errors from status code
      }
      else
      {
        Console.Write("Unexpected error getting invoices for order:");
        Console.Write(ex);
      }
      return new ListResponse<InvoiceDTO> { Success = false, Message = "Unknown error" };
    }
  }

  public async Task<ListResponse<InvoiceItemDTO>> GetItemsByInvoice(string invoiceId)
  {
    System.Net.HttpStatusCode? statusCode = null;
    try
    {
      var response = await _httpClient.GetAsync($"api/invoice/{Uri.EscapeDataString(invoiceId)}/items");
      statusCode = response.StatusCode;
      var result = await response.Content.ReadFromJsonAsync<ListResponse<InvoiceItemDTO>>();
      return result ?? new ListResponse<InvoiceItemDTO> { Success = false, Message = "Unknown error" };
    }
    catch (Exception ex)
    {
      if (statusCode is not null)
      {
        Console.Write($"Get invoices for order failed with status: {statusCode}");
        // TODO: Could narrow down errors from status code
      }
      else
      {
        Console.Write("Unexpected error getting invoices for order:");
        Console.Write(ex);
      }
      return new ListResponse<InvoiceItemDTO> { Success = false, Message = "Unknown error" };
    }
  }

  public async Task<BaseResponse> RateInvoiceItem(string invoiceId, string listingId, RatingChoice rating)
  {
    try
    {
      RatingDTO msg = new()
      {
        InvoiceId = invoiceId,
        ListingId = listingId,
        Rating = rating,
      };
      var response = await _httpClient.PostAsJsonAsync("api/invoice/rate", msg);

      var result = await response.Content.ReadFromJsonAsync<BaseResponse>();

      return result ?? new BaseResponse { Success = false, Message = "Unknown error" };
    }
    catch (Exception ex)
    {
      Console.WriteLine("Error setting order status:");
      Console.WriteLine(ex);
      return new BaseResponse { Success = false, Message = "Unknown error" };
    }
  }
}