using System;
using System.Net.Http.Json;
using System.Text.Json;
using GameVault.Shared.Models;

namespace GameVault.Client.Services;

public class ListingService
{
  private readonly HttpClient _httpClient;

  public ListingService(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public async Task<BaseResponse> CreateAsync(string name, decimal price, string description, int stock)
  {
    Listing newListing = new()
    {
      Id = "", // Fix later
      Name = name,
      Price = (int)(price * 100),
      Description = description,
      Stock = stock,
      Status = 0, // Add later
      OwnerID = "", // Add after login stuff
      Image = "" // Add later
    };
    var response = await _httpClient.PostAsJsonAsync("api/listing/create", newListing);
    var result = await response.Content.ReadFromJsonAsync<BaseResponse>();

    return result ?? new BaseResponse { Success = false, Message = "Unknown error" };
  }
}

public class BaseResponse
{
  public bool Success { get; set; }
  public string? Message { get; set; }
}