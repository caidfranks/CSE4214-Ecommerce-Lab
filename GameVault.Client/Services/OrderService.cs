using System.Net.Http.Json;
using GameVault.Shared.DTOs;
using GameVault.Shared.Models;

namespace GameVault.Client.Services;

public class OrderService
{
  private readonly HttpClient _httpClient;

  public OrderService(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public async Task<ListResponse<OrderDTO>> GetOrderHistory(string userId)
  {
    System.Net.HttpStatusCode? statusCode = null;
    try
    {
      var response = await _httpClient.GetAsync("api/orders");
      statusCode = response.StatusCode;
      var result = await response.Content.ReadFromJsonAsync<ListResponse<OrderDTO>>();
      return result ?? new ListResponse<OrderDTO> { Success = false, Message = "Unknown error" };
    }
    catch (Exception ex)
    {
      if (statusCode is not null)
      {
        Console.Write($"Get customer order history failed with status: {statusCode}");
        // TODO: Could narrow down errors from status code
      }
      else
      {
        Console.Write("Unexpected error getting customer order history:");
        Console.Write(ex);
      }
      return new ListResponse<OrderDTO> { Success = false, Message = "Unknown error" };
    }
  }
}