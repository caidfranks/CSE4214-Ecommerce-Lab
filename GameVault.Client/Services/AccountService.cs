using GameVault.Shared.DTOs;
using GameVault.Shared.Models;
using GameVaultWeb.Models;
using Grpc.Net.Client.Balancer;
using System;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace GameVault.Client.Services;

public class AccountService
{
  private readonly HttpClient _httpClient;

  public AccountService(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }
    public async Task<BaseResponse> ChangeAccountStatusToActive(string id)
    {
        var response = await _httpClient.PostAsJsonAsync("api/account/approve", id);
        var result = await response.Content.ReadFromJsonAsync<BaseResponse>();

        return result ?? new BaseResponse { Success = false, Message = "Unknown error" };
    }

    public async Task<BaseResponse> ChangeAccountStatusToDenied(string id)
    {
        var response = await _httpClient.PostAsJsonAsync("api/account/deny", id);
        var result = await response.Content.ReadFromJsonAsync<BaseResponse>();

        return result ?? new BaseResponse { Success = false, Message = "Unknown error" };
    }

    public async Task<BaseResponse> ChangeAccountStatusToBanned(string id)
    {
        var response = await _httpClient.PostAsJsonAsync("api/account/ban", id);
        var result = await response.Content.ReadFromJsonAsync<BaseResponse>();

        return result ?? new BaseResponse { Success = false, Message = "Unknown error" };
    }
    public async Task<AccountListResponse> GetAccountsByStatus(AccountStatus status)
    {
        var response = await _httpClient.GetAsync($"api/account/status?s={(int)status}");
        var result = await response.Content.ReadFromJsonAsync<AccountListResponse>();
        return result ?? new AccountListResponse { Success = false, Message = "Unknown error" };
    }
    public async Task<AccountDTO?> GetAccountByIdAsync(string id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/account/{id}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error fetching product: {response.StatusCode}");
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<AccountDTO>();

            if (result == null)
            {
                return null;
            }

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching product: {ex.Message}");
            return null;
        }
    }


    public async Task<BaseResponse> AddRemovalReason(string id, string reason)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"api/account/addRemovalReason/{id}", reason);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error fetching product: {response.StatusCode}");
                return null;
            }

            return await response.Content.ReadFromJsonAsync<BaseResponse>() ?? new BaseResponse { Success = false, Message = "No response from server" }; ;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching product: {ex.Message}");
            return null;
        }
    }
}