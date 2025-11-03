using GameVault.Shared.DTOs;
using GameVault.Shared.Models;
using GameVault.Client.Models;
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

    public async Task<BaseResponse> BanAccount(string id, string reason)
    {
        var response = await _httpClient.PostAsJsonAsync("api/account/ban", new BanUserDTO()
        {
            Id = id,
            BanMsg = reason
        });
        var result = await response.Content.ReadFromJsonAsync<BaseResponse>();

        return result ?? new BaseResponse { Success = false, Message = "Unknown error" };
    }
    public async Task<BaseResponse> UnbanAccount(string id)
    {
        var response = await _httpClient.PostAsJsonAsync("api/account/unban", new UnbanUserDTO()
        {
            Id = id
        });
        var result = await response.Content.ReadFromJsonAsync<BaseResponse>();

        return result ?? new BaseResponse { Success = false, Message = "Unknown error" };
    }
    public async Task<ListResponse<UserDTO>> GetVendorAccounts()
    {
        var response = await _httpClient.GetAsync("api/account/vendors");
        var result = await response.Content.ReadFromJsonAsync<ListResponse<UserDTO>>();
        return result ?? new ListResponse<UserDTO> { Success = false, Message = "Unknown error" };
    }
    public async Task<ListResponse<UserDTO>> GetCustomerAccounts()
    {
        var response = await _httpClient.GetAsync("api/account/customers");
        var result = await response.Content.ReadFromJsonAsync<ListResponse<UserDTO>>();
        return result ?? new ListResponse<UserDTO> { Success = false, Message = "Unknown error" };
    }
    public async Task<ListResponse<UserDTO>> GetBannedAccounts()
    {
        var response = await _httpClient.GetAsync("api/account/banned");
        var result = await response.Content.ReadFromJsonAsync<ListResponse<UserDTO>>();
        return result ?? new ListResponse<UserDTO> { Success = false, Message = "Unknown error" };
    }
    public async Task<ListResponse<RequestDTO>> GetVendorRequests()
    {
        var response = await _httpClient.GetAsync("api/account/requests");
        var result = await response.Content.ReadFromJsonAsync<ListResponse<RequestDTO>>();
        return result ?? new ListResponse<RequestDTO> { Success = false, Message = "Unknown error" };
    }
    public async Task<DataResponse<UserDTO>> GetAccountByIdAsync(string id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/account/id/{id}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error fetching user: {response.StatusCode}");
                return new DataResponse<UserDTO>()
                {
                    Success = false,
                    Message = "Unknown error"
                };
            }

            var result = await response.Content.ReadFromJsonAsync<DataResponse<UserDTO>>();

            if (result == null)
            {
                return new DataResponse<UserDTO>()
                {
                    Success = false,
                    Message = "Unknown error"
                };
            }

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching user: {ex.Message}");
            return new DataResponse<UserDTO>()
            {
                Success = false,
                Message = "Unknown error"
            };
        }
    }

    public async Task<BaseResponse> UpdateAccountAsync(UpdateAccountDTO dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync("api/account/update", dto);
            var result = await response.Content.ReadFromJsonAsync<BaseResponse>();

            return result ?? new BaseResponse
            {
                Success = false,
                Message = "Unknown error"
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating account: {ex.Message}");
            return new BaseResponse
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public async Task<BaseResponse> DeleteAccountAsync()
    {
        try
        {
            var response = await _httpClient.DeleteAsync("api/account/delete");
            var result = await response.Content.ReadFromJsonAsync<BaseResponse>();

            return result ?? new BaseResponse
            {
                Success = false,
                Message = "Unknown error"
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting account: {ex.Message}");
            return new BaseResponse
            {
                Success = false,
                Message = ex.Message
            };
        }
    }
}