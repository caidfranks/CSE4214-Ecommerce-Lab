using Microsoft.AspNetCore.Mvc;
using GameVault.Shared.Models;
using GameVault.Server.Services;
using GameVault.Shared.DTOs;
using GameVault.Server.Models;
using Google.Cloud.Firestore.V1;
using GameVault.Server.Models.Firestore;
using System.Runtime.InteropServices;

namespace GameVault.Server.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class LogController : ControllerBase
  {
    private readonly IFirestoreService _firestore;
    private readonly IConfiguration _configuration;

    public LogController(IFirestoreService firestore, IConfiguration configuration)
    {
      _firestore = firestore;
      _configuration = configuration;
    }

    [HttpGet]
    public async Task<ActionResult<LogListResponse>> GetAllLogs()
    {
      var apiKey = _configuration["Firebase:ApiKey"];
      if (string.IsNullOrEmpty(apiKey))
      {
        return StatusCode(500, new LogListResponse
        {
          Success = false,
          Message = "Firebase configuration error"
        });
      }

      try

      {
        var logs = await _firestore.GetCollectionAsyncWithId<Log>("logs");

        List<LogDTO> logDTOs = [];
        foreach (Log log in logs)
        {
          LogDTO logDTO = new()
          {
            Id = log.Id,
            Summary = log.Summary,
            Type = log.Type,
            ObjectId = log.ObjectId,
            Status = log.Status,
            Timestamp = log.Timestamp,
            Details = log.Details
          };
          logDTOs.Add(logDTO);
        }

        return new LogListResponse
        {
          Success = true,
          Logs = logDTOs
        };
      }
      catch (Exception ex)
      {
        return StatusCode(500, new LogListResponse { Success = false, Message = ex.Message });
      }
    }
  }
}
