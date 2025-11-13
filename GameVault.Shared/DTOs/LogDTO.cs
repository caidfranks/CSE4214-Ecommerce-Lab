namespace GameVault.Shared.Models;

public class LogDTO
{
  public required string Id { get; set; }
  public required string Summary { get; set; }
  public required LogType Type { get; set; }
  public required string ObjectId { get; set; }
  public required int Status { get; set; }
  public required DateTime Timestamp { get; set; }
  public string? Details { get; set; }
}