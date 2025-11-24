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
        private readonly NotificationService _notifService;
        private readonly IConfiguration _configuration;

        public ListingController(IFirebaseAuthService firebaseAuth, IFirestoreService firestore, UserService userService, NotificationService notifService, IConfiguration configuration)
        {
            _firebaseAuth = firebaseAuth;
            _firestore = firestore;
            _userService = userService;
            _notifService = notifService;
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
                return Unauthorized();
            }
            else if (user.Type != AccountType.Vendor)
            {
                return Forbid();
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
                Category = newListing.Category,
                Image = newListing.Image
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
                    Category = listing.Category,
                    Rating = listing.Rating,
                    NumReviews = listing.NumReviews
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
                    Category = listing.Category,
                    Rating = listing.Rating,
                    NumReviews = listing.NumReviews
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
            var adminUsers = await _firestore.QueryDocumentsAsyncWithId<User>("users", "Type", (int)AccountType.Admin);
            foreach (var admin in adminUsers)
            {
                await _notifService.CreateNotifAsync(admin.Id, "New Listing Application", "A new listing has been submitted");
            }

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
            var listing = await _firestore.GetDocumentAsync<Listing>("listings", id);
            await _notifService.CreateNotifAsync(listing.OwnerID, "Listing Inactive", "Your listing has been made inactive.");


            // TODO: Handle firestore errors

            return Ok(new BaseResponse
            {
                Success = true,
                Message = "Listing status successfully updated to pending",
            });
        }

        [HttpPost("deactivate")]
        public async Task<ActionResult<BaseResponse>> DeactivateAllUserListingsAsync([FromBody] string userId)
        {
            try
            {
                // Get all listings owned by the specified user
                var listings = await _firestore.QueryDocumentsAsyncWithId<Listing>("listings", "OwnerID", userId);

                if (listings == null || listings.Count == 0)
                {
                    return new BaseResponse
                    {
                        Success = false,
                        Message = "No listings found for this user."
                    };
                }

                // Set each listing to inactive
                foreach (var listing in listings)
                {
                    await _firestore.SetDocumentFieldAsync("listings", listing.Id, "Status", (int)ListingStatus.Inactive);
                }

                return new BaseResponse
                {
                    Success = true,
                    Message = $"All {listings.Count} listings for user {userId} have been set to inactive."
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponse
                {
                    Success = false,
                    Message = $"Error deactivating listings: {ex.Message}"
                });
            }
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
            var listing = await _firestore.GetDocumentAsync<Listing>("listings", id);
            await _notifService.CreateNotifAsync(listing.OwnerID, "Listing Approved", "Your listing has been approved.");

            // TODO: Handle firestore errors

            return Ok(new BaseResponse
            {
                Success = true,
                Message = "Listing status successfully updated to pending",
            });
        }

        [HttpPost("remove")]
        public async Task<ActionResult<BaseResponse>> ChangeListingStatusToRemoved([FromBody] string id)
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

            await _firestore.SetDocumentFieldAsync("listings", id, "Status", (int)ListingStatus.Removed);
            var listing = await _firestore.GetDocumentAsync<Listing>("listings", id);
            await _notifService.CreateNotifAsync(listing.OwnerID, "Listing Removed", "Your listing has been removed.");

            // TODO: Handle firestore errors

            return Ok(new BaseResponse
            {
                Success = true,
                Message = "Listing status successfully updated to removed",
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
                listing.Image = modListing.Image;
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

        [HttpPost("{listingId}/upload-image")]
        public async Task<ActionResult<DataResponse<string>>> UploadListingImage(
    string listingId,
    IFormFile file,
    [FromHeader] string? Authorization)
        {
            try
            {
                // Verify user is authenticated
                var user = await _userService.GetUserFromHeader(Authorization);
                if (user == null)
                {
                    return Unauthorized(new DataResponse<string>
                    {
                        Success = false,
                        Message = "Not authenticated"
                    });
                }

                // Get the listing to verify ownership
                var listing = await _firestore.GetDocumentAsync<FirestoreListing>("listings", listingId);
                if (listing == null)
                {
                    return NotFound(new DataResponse<string>
                    {
                        Success = false,
                        Message = "Listing not found"
                    });
                }

                // Verify user owns this listing or is admin
                if (listing.OwnerID != user.Id && user.Type != AccountType.Admin)
                {
                    return Forbid();
                }

                // Validate file
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new DataResponse<string>
                    {
                        Success = false,
                        Message = "No file uploaded"
                    });
                }

                // Validate file type
                var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp", "image/jpg" };
                if (!allowedTypes.Contains(file.ContentType.ToLower()))
                {
                    return BadRequest(new DataResponse<string>
                    {
                        Success = false,
                        Message = "Only JPEG, PNG, and WebP images are allowed"
                    });
                }

                // Validate file size (500KB max)
                if (file.Length > 500 * 1024)
                {
                    return BadRequest(new DataResponse<string>
                    {
                        Success = false,
                        Message = "File size must be less than 500KB"
                    });
                }

                // Convert to Base64
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                var bytes = memoryStream.ToArray();
                var base64 = Convert.ToBase64String(bytes);
                var dataUrl = $"data:{file.ContentType};base64,{base64}";

                // Update listing in Firestore
                await _firestore.SetDocumentFieldAsync("listings", listingId, "Image", dataUrl);

                return Ok(new DataResponse<string>
                {
                    Success = true,
                    Message = "Image uploaded successfully",
                    Data = dataUrl
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading image: {ex.Message}");
                return StatusCode(500, new DataResponse<string>
                {
                    Success = false,
                    Message = "Failed to upload image"
                });
            }
        }
    }
}

public class BaseResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
}

