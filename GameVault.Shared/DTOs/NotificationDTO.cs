using GameVault.Shared.Models;
namespace GameVault.Shared.DTOs;

public class NotificationDTO
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
}