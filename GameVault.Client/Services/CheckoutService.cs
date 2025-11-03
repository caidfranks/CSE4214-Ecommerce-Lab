using GameVault.Shared.DTOs;
using GameVault.Shared.Models;
using System.Net.Http.Json;

namespace GameVault.Client.Services;

public class CheckoutService
{
    private readonly HttpClient _http;
    private readonly ILogger<CheckoutService> _logger;

    public CheckoutService(HttpClient http, ILogger<CheckoutService> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<CheckoutResponseDTO> ProcessCheckoutAsync(CheckoutRequestDTO request)
    {
        try
        {
            _logger.LogInformation("Processing checkout");

            var response = await _http.PostAsJsonAsync("/api/checkout", request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CheckoutResponseDTO>();
                return result ?? new CheckoutResponseDTO
                {
                    Success = false,
                    ErrorMessage = "Invalid response from server"
                };
            }
            else
            {
                var errorResult = await response.Content.ReadFromJsonAsync<CheckoutResponseDTO>();
                return errorResult ?? new CheckoutResponseDTO
                {
                    Success = false,
                    ErrorMessage = $"Checkout failed: {response.StatusCode}"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during checkout");
            return new CheckoutResponseDTO
            {
                Success = false,
                ErrorMessage = $"An error occurred: {ex.Message}"
            };
        }
    }

    public async Task<EstimateTaxResponse?> EstimateTaxAsync(int subtotalInCents, string state)
    {
        try
        {
            var request = new EstimateTaxRequest
            {
                SubtotalInCents = subtotalInCents,
                State = state
            };

            var response = await _http.PostAsJsonAsync("/api/checkout/estimate-tax", request);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<EstimateTaxResponse>();
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error estimating tax");
            return null;
        }
    }

    public async Task<Order?> GetOrderAsync(string orderId)
    {
        try
        {
            return await _http.GetFromJsonAsync<Order>($"/api/orders/{orderId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order {orderId}", orderId);
            return null;
        }
    }

    public async Task<List<Order>> GetMyOrdersAsync()
    {
        try
        {
            var orders = await _http.GetFromJsonAsync<List<Order>>("/api/orders");
            return orders ?? new List<Order>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting orders");
            return new List<Order>();
        }
    }

    public async Task<bool> CancelOrderAsync(string orderId)
    {
        try
        {
            var response = await _http.PostAsync($"/api/orders/{orderId}/cancel", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling order {orderId}", orderId);
            return false;
        }
    }
}

public class EstimateTaxRequest
{
    public required int SubtotalInCents { get; set; }
    public required string State { get; set; }
}

public class EstimateTaxResponse
{
    public int SubtotalInCents { get; set; }
    public int TaxInCents { get; set; }
    public int TotalInCents { get; set; }
    public decimal TaxRate { get; set; }
}

public class Order
{
    public string Id { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public int SubtotalInCents { get; set; }
    public int TaxInCents { get; set; }
    public int TotalInCents { get; set; }
}