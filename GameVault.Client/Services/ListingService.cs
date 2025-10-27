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
      OwnerID = "A6vUMHOrUIWOiFXw2pPPm7yMLHD2", // Add after login stuff
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

    public async Task<BaseResponse> ChangeListingStatusToPublished(string id)
    {
        var response = await _httpClient.PostAsJsonAsync("api/listing/approve", id);
        var result = await response.Content.ReadFromJsonAsync<BaseResponse>();

        return result ?? new BaseResponse { Success = false, Message = "Unknown error" };
    }

    public async Task<ListingListResponse> GetVendorListingsByStatus(string userId, ListingStatus status)
  {
    var response = await _httpClient.GetAsync($"api/listing/vendor?v={Uri.EscapeDataString(userId)}&s={status}");
    var result = await response.Content.ReadFromJsonAsync<ListingListResponse>();
    return result ?? new ListingListResponse { Success = false, Message = "Unknown error" };
  }

    public async Task<ListingListResponse> GetListingsByStatus(ListingStatus status)
    {
        var response = await _httpClient.GetAsync($"api/listing/status?s={status}");
        var result = await response.Content.ReadFromJsonAsync<ListingListResponse>();
        return result ?? new ListingListResponse { Success = false, Message = "Unknown error" };
    }
}