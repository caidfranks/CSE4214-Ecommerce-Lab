using System;
using System.Net.Http.Json;
using System.Text.Json;
using GameVault.Shared.Models;
using GameVault.Shared.DTOs;
using Grpc.Net.Client.Balancer;
using System.Net;

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
      var response = await _httpClient.GetAsync("api/logs");

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
}