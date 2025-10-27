using System;
using System.Net.Http.Json;
using System.Text.Json;
using GameVault.Shared.Models;
using GameVault.Shared.DTOs;
using Grpc.Net.Client.Balancer;
using System.Net;

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
    NewListingDTO newListing = new()
    {
      Name = name,
      Price = (int)(price * 100),
      Description = description,
      Stock = stock,
      Status = ListingStatus.Inactive,
      // Image = "" // Add later
    };
    var response = await _httpClient.PostAsJsonAsync("api/listing/create", newListing);
    var result = await response.Content.ReadFromJsonAsync<BaseResponse>();

    return result ?? new BaseResponse { Success = false, Message = "Unknown error" };
  }

  public async Task<BaseResponse> ChangeListingStatusToPending(string id)
  {
    var response = await _httpClient.PostAsJsonAsync("api/listing/submit", id);
    var result = await response.Content.ReadFromJsonAsync<BaseResponse>();

    return result ?? new BaseResponse { Success = false, Message = "Unknown error" };
  }

  public async Task<BaseResponse> ChangeListingStatusToInactive(string id)
  {
    var response = await _httpClient.PostAsJsonAsync("api/listing/cancel", id);
    var result = await response.Content.ReadFromJsonAsync<BaseResponse>();

    return result ?? new BaseResponse { Success = false, Message = "Unknown error" };
  }

  public async Task<VendorListingListResponse> GetVendorListingsByStatus(string userId, ListingStatus status)
  {
    var response = await _httpClient.GetAsync($"api/listing/vendor?v={Uri.EscapeDataString(userId)}&s={status}");
    var result = await response.Content.ReadFromJsonAsync<VendorListingListResponse>();
    return result ?? new VendorListingListResponse { Success = false, Message = "Unknown error" };
  }

  public async Task<ListingResponse> GetListingById(string listingId)
  {
    var response = await _httpClient.GetAsync($"api/listing/id?id={Uri.EscapeDataString(listingId)}");
    var result = await response.Content.ReadFromJsonAsync<ListingResponse>();
    return result ?? new ListingResponse { Success = false, Message = "Unknown error" };
  }

  public async Task<BaseResponse> UpdateAsync(string id, string name, string desc, decimal price, int stock)
  {
    ListingDTO newListing = new()
    {
      Id = id,
      OwnerID = "",
      LastModified = DateTime.UtcNow,
      Name = name,
      Price = (int)(price * 100),
      Description = desc,
      Stock = stock,
      Status = ListingStatus.Inactive,
      // Image = "" // Add later
    };
    var response = await _httpClient.PostAsJsonAsync("api/listing/update", newListing);
    // TODO: Implement this logic everywhere to protect against errors in ReadFromJsonAsync
    if (response.StatusCode != HttpStatusCode.OK)
    {
      return new BaseResponse
      {
        Success = false,
        Message = "Http error"
      };
    }
    var result = await response.Content.ReadFromJsonAsync<BaseResponse>();

    return result ?? new BaseResponse { Success = false, Message = "Unknown error" };
  }

  public async Task<BaseResponse> UpdateStockAsync(string id, int newStock)
  {
    ListingStockDTO stockDTO = new()
    {
      Id = id,
      Stock = newStock
    };
    var response = await _httpClient.PostAsJsonAsync("api/listing/stock", stockDTO);
    if (response.StatusCode != HttpStatusCode.OK)
    {
      return new BaseResponse
      {
        Success = false,
        Message = "Http error"
      };
    }
    var result = await response.Content.ReadFromJsonAsync<BaseResponse>();

    return result ?? new BaseResponse { Success = false, Message = "Unknown error" };
  }
}