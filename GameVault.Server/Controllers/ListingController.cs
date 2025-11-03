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
    public class ListingController : ControllerBase
    {
        private readonly IFirebaseAuthService _firebaseAuth;
        private readonly IFirestoreService _firestore;
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;

        public ListingController(IFirebaseAuthService firebaseAuth, IFirestoreService firestore, UserService userService, IConfiguration configuration)
        {
            _firebaseAuth = firebaseAuth;
            _firestore = firestore;
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost("create")]
        public async Task<ActionResult<BaseResponse>> Create([FromBody] NewListingDTO newListing, [FromHeader] string? Authorization)
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

            var user = await _userService.GetUserFromHeader(Authorization);

            if (user is null)
            {
                return Forbid();
            }
            else if (user.Type != AccountType.Vendor)
            {
                return Unauthorized();
            }

            FirestoreListing newListingObj = new()
            {
                // Id = "",
                Name = newListing.Name,
                Description = newListing.Description,
                Price = newListing.Price,
                Stock = newListing.Stock,
                Status = newListing.Status,
                OwnerID = user.Id,
                LastModified = DateTime.UtcNow,
                Category = newListing.Category
            };

            await _firestore.AddDocumentAsync("listings", newListingObj);

            return Ok(new BaseResponse
            {
                Success = true,
                Message = "New Listing created successfully",
            });
        }

        [HttpGet("vendor")]
        public async Task<ActionResult<VendorListingListResponse>> GetVendorListingsByStatus([FromQuery] string v, [FromQuery] ListingStatus s, [FromHeader] string Authorization)
        {
            // Console.WriteLine($"Got query for User {v} with status {s}");
            var apiKey = _configuration["Firebase:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                return StatusCode(500, new VendorListingListResponse
                {
                    Success = false,
                    Message = "Firebase configuration error"
                });
            }

            var user = await _userService.GetUserFromHeader(Authorization);

            if (user is null)
            {
                return Forbid();
            }
            else if (user.Type == AccountType.Customer)
            {
                return Unauthorized(new VendorListingListResponse
                {
                    Success = false,
                    Message = "Must be a vendor to view this page",
                });
            }

            Console.WriteLine(user?.Type.ToString() ?? "Not logged in");

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

            List<VendorListingDTO> listingDTOs = [];

            // Console.WriteLine($"Controller got {listings.Count} listings");

            foreach (Models.Firestore.Listing listing in listings)
            {
                VendorListingDTO listingDTO = new()
                {
                    RemoveMsg = listing.RemoveMsg,
                    Id = listing.Id,
                    Name = listing.Name,
                    Price = listing.Price,
                    Description = listing.Description,
                    Stock = listing.Stock,
                    Status = listing.Status,
                    OwnerID = listing.OwnerID,
                    Image = listing.Image,
                    LastModified = listing.LastModified,
                    Category = listing.Category
                };
                listingDTOs.Add(listingDTO);
            }

            // Console.WriteLine($"Controller returning {listingDTOs.Count} listings");

            return new VendorListingListResponse { Success = true, Listings = listingDTOs };

            // return new ListingListResponse { Success = false, Message = "Not configured" };
        }

        [HttpGet("status")]
        public async Task<ActionResult<ListResponse<VendorListingDTO>>> GetListingsByStatus([FromQuery] ListingStatus s)
        {
            var apiKey = _configuration["Firebase:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                return StatusCode(500, new ListingListResponse
                {
                    Success = false,
                    Message = "Firebase configuration error"
                });
            }

            var listings = await _firestore.QueryComplexDocumentsAsyncWithId<Models.Firestore.Listing>(
                "listings",
                [
                    new() {
                fieldName = "Status",
                value = (int)s
            }
                ]
            );

            List<VendorListingDTO> listingDTOs = [];

            foreach (var listing in listings)
            {
                VendorListingDTO listingDTO = new()
                {
                    RemoveMsg = listing.RemoveMsg,
                    Id = listing.Id,
                    Name = listing.Name,
                    Price = listing.Price,
                    Description = listing.Description,
                    Stock = listing.Stock,
                    Status = listing.Status,
                    OwnerID = listing.OwnerID,
                    Image = listing.Image,
                    LastModified = listing.LastModified,
                    Category = listing.Category
                };
                listingDTOs.Add(listingDTO);
            }

            //Console.WriteLine($"Controller returning {listingDTOs.Count} listings");

            return new ListResponse<VendorListingDTO> { Success = true, List = listingDTOs };

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

        [HttpPost("approve")]
        public async Task<ActionResult<BaseResponse>> ChangeListingStatusToPublished([FromBody] string id)
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

            await _firestore.SetDocumentFieldAsync("listings", id, "Status", (int)ListingStatus.Published);

            // TODO: Handle firestore errors

            return Ok(new BaseResponse
            {
                Success = true,
                Message = "Listing status successfully updated to pending",
            });
        }

        [HttpGet("id")]
        public async Task<ActionResult<ListingResponse>> GetListingById([FromQuery] string id)
        {
            // TODO: Refactor this so not needed in every single API call
            var apiKey = _configuration["Firebase:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                return StatusCode(500, new BaseResponse
                {
                    Success = false,
                    Message = "Firebase configuration error"
                });
            }

            FirestoreListing? listing = await _firestore.GetDocumentAsync<FirestoreListing>("listings", id);

            // TODO: After Server Auth Permanence, Do Security
            if (listing is not null) // && (Listing.Status == ListingStatus.Published || Listing.OwnerId == UserId)
            {
                if (listing.Status == ListingStatus.Inactive || listing.Status == ListingStatus.Removed)
                {
                    return Ok(new ListingResponse
                    {
                        Success = true,
                        Listing = new()
                        {
                            Id = id,
                            OwnerID = listing.OwnerID,
                            LastModified = listing.LastModified,
                            Name = listing.Name,
                            Price = listing.Price,
                            Description = listing.Description,
                            Stock = listing.Stock,
                            Status = listing.Status,
                            Image = listing.Image,
                            Category = listing.Category
                        }
                    });
                }
                else
                {
                    return StatusCode(403, new ListingResponse
                    {
                        Success = false,
                        Message = "You must unpublish this listing before editing"
                    });
                }
            }
            else
            {
                return StatusCode(404, new ListingResponse
                {
                    Success = false,
                    Message = "Listing not found"
                });
            }
        }

        [HttpPost("update")]
        public async Task<ActionResult<BaseResponse>> Update([FromBody] ListingDTO modListing)
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

            // Get listing
            FirestoreListing? listing = await _firestore.GetDocumentAsync<FirestoreListing>("listings", modListing.Id);

            if (listing is null)
            {
                return StatusCode(404, new BaseResponse
                {
                    Success = false,
                    Message = "Listing not found"
                });
            }

            // TODO: Make sure owner

            // Make sure status is editable
            if (listing.Status == ListingStatus.Inactive || listing.Status == ListingStatus.Removed)
            {
                // Change fields based on DTO
                listing.Name = modListing.Name;
                listing.Description = modListing.Description;
                listing.Price = modListing.Price;
                listing.Stock = modListing.Stock;
                listing.Status = ListingStatus.Inactive;
                // Update in Firestore
                await _firestore.SetDocumentAsync("listings", modListing.Id, listing);

                return Ok(new BaseResponse
                {
                    Success = true,
                    Message = "New Listing created successfully"
                });
            }
            else
            {
                return StatusCode(403, new BaseResponse
                {
                    Success = false,
                    Message = "You must unpublish this listing before editing"
                });
            }
        }

        [HttpPost("stock")]
        public async Task<ActionResult<BaseResponse>> UpdateStock([FromBody] ListingStockDTO stockDTO)
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
            // Make sure within valid range of stock

            await _firestore.SetDocumentFieldAsync("listings", stockDTO.Id, "Stock", stockDTO.Stock);

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

