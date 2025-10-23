using Microsoft.AspNetCore.Mvc;
using GameVault.Shared.Models;
using GameVault.Server.Services;
using GameVault.Shared.DTOs;
using GameVault.Server.Models;
using Google.Cloud.Firestore.V1;
using GameVault.Server.Models.Firestore;

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
        public async Task<ActionResult<BaseResponse>> Create([FromBody] NewListingDTO newListing)
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

            FirestoreListing newListingObj = new()
            {
                // Id = "",
                Name = newListing.Name,
                Description = newListing.Description,
                Price = newListing.Price,
                Stock = newListing.Stock,
                Status = newListing.Status,
                // TODO: User ID not accessible on server - huge security risk
                OwnerID = "Fo3LIgVvpBrteVibSry2smUk2nTn",
                LastModified = DateTime.UtcNow
            };

            await _firestore.AddDocumentAsync("listings", newListingObj);

            return Ok(new BaseResponse
            {
                Success = true,
                Message = "New Listing created successfully",
            });
        }

        [HttpGet("vendor")]
        public async Task<ActionResult<ListingListResponse>> GetVendorListingsByStatus([FromQuery] string v, [FromQuery] ListingStatus s)
        {
            // Console.WriteLine($"Got query for User {v} with status {s}");
            var apiKey = _configuration["Firebase:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                return StatusCode(500, new ListingListResponse
                {
                    Success = false,
                    Message = "Firebase configuration error"
                });
            }

            var listings = await _firestore.QueryComplexDocumentsAsyncWithId<Models.Firestore.Listing>("listings",
            [
                new() {
                    fieldName = "OwnerID",
                    value = v
                },
                new() {
                    fieldName = "Status",
                    value = (int)s
                }
            ]);

            List<ListingDTO> listingDTOs = [];

            Console.WriteLine($"Controller got {listings.Count} listings");

            foreach (Models.Firestore.Listing listing in listings)
            {
                ListingDTO listingDTO = new()
                {
                    Id = listing.Id,
                    Name = listing.Name,
                    Price = listing.Price,
                    Description = listing.Description,
                    Stock = listing.Stock,
                    Status = listing.Status,
                    OwnerID = listing.OwnerID,
                    Image = listing.Image,
                    LastModified = listing.LastModified
                };
                listingDTOs.Add(listingDTO);
            }

            Console.WriteLine($"Controller returning {listingDTOs.Count} listings");

            return new ListingListResponse { Success = true, Listings = listingDTOs };

            // return new ListingListResponse { Success = false, Message = "Not configured" };
        }

        [HttpPost("submit")]
        public async Task<ActionResult<BaseResponse>> ChangeListingStatusToPending([FromBody] string id)
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

            // TODO: Make sure owner

            await _firestore.SetDocumentFieldAsync("listings", id, "Status", (int)ListingStatus.Pending);

            // TODO: Handle firestore errors

            return Ok(new BaseResponse
            {
                Success = true,
                Message = "Listing status successfully updated to pending",
            });
        }


        [HttpPost("cancel")]
        public async Task<ActionResult<BaseResponse>> ChangeListingStatusToInactive([FromBody] string id)
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

            // TODO: Make sure owner

            await _firestore.SetDocumentFieldAsync("listings", id, "Status", (int)ListingStatus.Inactive);

            // TODO: Handle firestore errors

            return Ok(new BaseResponse
            {
                Success = true,
                Message = "Listing status successfully updated to pending",
            });
        }
    }
}

public class BaseResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
}

