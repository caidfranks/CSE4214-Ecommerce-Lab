using System.Net.Http.Json;
using GameVault.Shared.DTOs;
using GameVault.Shared.Models;
using System.Xml.Linq;
using System.Net.Http;

namespace GameVault.Client.Services;

public class NotificationService
{
    private readonly HttpClient _http;

    public NotificationService(HttpClient http)
    {
        _http = http;
    }

    public async Task<ListResponse<NotificationDTO>> GetNotifsAsync(string userId)
    {
        return await _http.GetFromJsonAsync<ListResponse<NotificationDTO>>($"api/notification/{userId}") ?? new ListResponse<NotificationDTO> { Success = false };
    }
}