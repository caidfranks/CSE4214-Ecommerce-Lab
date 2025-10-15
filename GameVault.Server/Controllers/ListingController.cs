using Microsoft.AspNetCore.Mvc;
using GameVault.Shared.Models;
using GameVault.Server.Services;
using OneOf.Types;
using FirebaseAdmin.Messaging;

namespace GameVault.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ListingController : ControllerBase
    {
        private readonly IFirestoreService _firestore;
        private readonly IConfiguration _configuration;

        public ListingController(IFirestoreService firestore, IConfiguration configuration)
        {
            _firestore = firestore;
            _configuration = configuration;
        }

        [HttpPost("create")]
        public async Task<ActionResult<BaseResponse>> Create([FromBody] Listing newListing)
        {
            var apiKey = _configuration["Firebase:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                return StatusCode(500, new BaseResponse
                {
                    Success = false,
                    Message = "Firebase configuration error"
                });
            }

            await _firestore.AddDocumentAsync("listings", newListing);

            return Ok(new BaseResponse
            {
                Success = true,
                Message = "New Listing created successfully",
            });
        }
    }
}

public class BaseResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
}

