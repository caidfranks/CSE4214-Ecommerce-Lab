using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace GameVault.Server.Services;

public class SquarePaymentService
{
    private readonly ILogger<SquarePaymentService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _accessToken;
    private readonly string _locationId;

    public SquarePaymentService(IConfiguration configuration, ILogger<SquarePaymentService> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();

        _accessToken = configuration["Square:AccessToken"]
            ?? throw new InvalidOperationException("Square:AccessToken not configured");
        _locationId = configuration["Square:LocationId"]
            ?? throw new InvalidOperationException("Square:LocationId not configured");

        _httpClient.BaseAddress = new Uri("https://connect.squareupsandbox.com/v2/");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        _logger.LogInformation("SquarePaymentService initialized in SANDBOX mode");
    }

    public async Task<(bool success, string? paymentId, string? errorMessage)> ProcessPaymentAsync(
        int amountInCents,
        string paymentMethodId,
        string customerId,
        string orderId)
    {
        try
        {
            var requestBody = new
            {
                source_id = paymentMethodId,
                idempotency_key = Guid.NewGuid().ToString(),
                amount_money = new
                {
                    amount = amountInCents,
                    currency = "USD"
                },
                location_id = _locationId,
                reference_id = orderId,
                note = $"GameVault Order {orderId}"
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("payments", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<JsonElement>(responseBody);
                var payment = result.GetProperty("payment");
                var paymentId = payment.GetProperty("id").GetString();
                var status = payment.GetProperty("status").GetString();

                if (status == "COMPLETED")
                {
                    _logger.LogInformation("Payment successful! Square Payment ID: {paymentId}", paymentId);
                    return (true, paymentId, null);
                }
                else
                {
                    return (false, null, $"Payment status: {status}");
                }
            }
            else
            {
                _logger.LogError("Square API error: {status} - {body}", response.StatusCode, responseBody);
                return (false, null, $"Square API error: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Payment processing failed for order {orderId}", orderId);
            return (false, null, ex.Message);
        }
    }

    public async Task<(bool success, string? refundId, string? errorMessage)> RefundPaymentAsync(
        string paymentId,
        int amountInCents,
        string reason)
    {
        try
        {
            var requestBody = new
            {
                idempotency_key = Guid.NewGuid().ToString(),
                amount_money = new
                {
                    amount = amountInCents,
                    currency = "USD"
                },
                payment_id = paymentId,
                reason = reason
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("refunds", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<JsonElement>(responseBody);
                var refund = result.GetProperty("refund");
                var refundId = refund.GetProperty("id").GetString();
                var status = refund.GetProperty("status").GetString();

                if (status == "COMPLETED" || status == "PENDING")
                {
                    _logger.LogInformation("Refund successful! Square Refund ID: {refundId}, Status: {status}", refundId, status);
                    return (true, refundId, null);
                }
                else
                {
                    return (false, null, $"Refund status: {status}");
                }
            }
            else
            {
                _logger.LogError("Square API error: {status} - {body}", response.StatusCode, responseBody);
                return (false, null, $"Square API error: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Refund processing failed for payment {paymentId}", paymentId);
            return (false, null, ex.Message);
        }
    }

    public async Task<(bool success, string? errorMessage)> VerifyPaymentAsync(string paymentId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"payments/{paymentId}");
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<JsonElement>(responseBody);
                var payment = result.GetProperty("payment");
                var status = payment.GetProperty("status").GetString();

                if (status == "COMPLETED")
                {
                    return (true, null);
                }
                else
                {
                    return (false, $"Payment status: {status}");
                }
            }
            else
            {
                _logger.LogError("Square API error: {status} - {body}", response.StatusCode, responseBody);
                return (false, $"Square API error: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Payment verification failed for {paymentId}", paymentId);
            return (false, ex.Message);
        }
    }
}