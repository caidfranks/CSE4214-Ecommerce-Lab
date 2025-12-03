using GameVault.Shared.DTOs;
using GameVault.Shared.Models;
using Grpc.Net.Client.Balancer;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace GameVault.Client.Services;

public class ListingService
{
  private readonly HttpClient _httpClient;

  public ListingService(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public async Task<BaseResponse> CreateAsync(string name, decimal price, string description, int stock, string category, string image)
  {
    NewListingDTO newListing = new()
    {
      Name = name,
      Price = (int)(price * 100),
      Description = description,
      Stock = stock,
      Status = ListingStatus.Inactive,
      Category = category,
      Image = image
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

  public async Task<BaseResponse> DeactivateAllUserListingsAsync(string userId)
  {
    var response = await _httpClient.PostAsJsonAsync("api/listing/deactivate", userId);
    var result = await response.Content.ReadFromJsonAsync<BaseResponse>();

    return result ?? new BaseResponse
    {
      Success = false,
      Message = "Unknown error"
    };
  }

  public async Task<BaseResponse> ChangeListingStatusToPublished(string id)
  {
    var response = await _httpClient.PostAsJsonAsync("api/listing/approve", id);
    var result = await response.Content.ReadFromJsonAsync<BaseResponse>();

    return result ?? new BaseResponse { Success = false, Message = "Unknown error" };
  }

  public async Task<BaseResponse> ChangeListingStatusToRemoved(string id)
  {
    var response = await _httpClient.PostAsJsonAsync("api/listing/remove", id);
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

  public async Task<BaseResponse> UpdateAsync(string id, string name, string desc, decimal price, int stock, string category, string image)
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
      Category = category,
      Image = image
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

  public async Task<BaseResponse> UpdateStockAsync(string id, int addStock)
  {
    ListingStockDTO stockDTO = new()
    {
      Id = id,
      AddStock = addStock
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

  public async Task<ListResponse<VendorListingDTO>> GetListingsByStatus(ListingStatus status)
  {
    var response = await _httpClient.GetAsync($"api/listing/status?s={status}");
    var result = await response.Content.ReadFromJsonAsync<ListResponse<VendorListingDTO>>();
    return result ?? new ListResponse<VendorListingDTO> { Success = false, Message = "Unknown error" };
  }

  public async Task<DataResponse<string>> UploadListingImageAsync(string listingId, IBrowserFile file)
  {
    try
    {
      using var content = new MultipartFormDataContent();
      var fileContent = new StreamContent(file.OpenReadStream(maxAllowedSize: 500 * 1024));
      fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
      content.Add(fileContent, "file", file.Name);

      var response = await _httpClient.PostAsync($"api/listing/{listingId}/upload-image", content);
      var result = await response.Content.ReadFromJsonAsync<DataResponse<string>>();

      return result ?? new DataResponse<string> { Success = false, Message = "Unknown error" };
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error uploading image: {ex.Message}");
      return new DataResponse<string>
      {
        Success = false,
        Message = "Failed to upload image"
      };
    }
  }
}