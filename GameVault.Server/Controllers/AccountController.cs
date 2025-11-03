using GameVault.Server.Models;
using GameVault.Server.Models.Firestore;
using GameVault.Server.Services;
using GameVault.Shared.DTOs;
using GameVault.Shared.Models;
using Google.Cloud.Firestore.V1;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static Google.Rpc.Context.AttributeContext.Types;

namespace GameVault.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IFirestoreService _firestore;
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;

        public AccountController(IFirestoreService firestore, UserService userService, IConfiguration configuration)
        {
            _firestore = firestore;
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost("archive")]
        public async Task<ActionResult<BaseResponse>> ArchiveRequest([FromBody] string id)
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

            await _firestore.SetDocumentFieldAsync("requests", id, "Archived", true);

            // TODO: Handle firestore errors

            return Ok(new BaseResponse
            {
                Success = true,
                Message = "Listing status successfully updated to pending",
            });
        }

        [HttpPost("ban")]
        public async Task<ActionResult<BaseResponse>> ChangeAccountStatusToBanned([FromBody] BanUserDTO dto, [FromHeader] string? Authorization)
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

            // Make sure admin
            var user = await _userService.GetUserFromHeader(Authorization);

            if (user is null)
            {
                return Unauthorized();
            }
            else if (user.Type != AccountType.Admin)
            {
                return Forbid();
            }

            await _firestore.SetDocumentFieldAsync("users", dto.Id, "BanMsg", dto.BanMsg);
            await _firestore.SetDocumentFieldAsync("users", dto.Id, "Banned", true);

            // TODO: Handle firestore errors

            return Ok(new BaseResponse
            {
                Success = true,
                Message = "Listing status successfully updated to pending",
            });
        }

        [HttpPost("unban")]
        public async Task<ActionResult<BaseResponse>> UnbanUser([FromBody] UnbanUserDTO dto, [FromHeader] string? Authorization)
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

            // Make sure admin
            var user = await _userService.GetUserFromHeader(Authorization);

            if (user is null)
            {
                return Unauthorized();
            }
            else if (user.Type != AccountType.Admin)
            {
                return Forbid();
            }

            await _firestore.SetDocumentFieldAsync("users", dto.Id, "Banned", false);

            // TODO: Handle firestore errors

            return Ok(new BaseResponse
            {
                Success = true,
                Message = "User successfully unbanned",
            });
        }

        [HttpGet("id/{id}")]
        public async Task<ActionResult<DataResponse<UserDTO>>> GetAccountById(string id, [FromHeader] string? Authorization)
        {
            // Make sure admin
            var user = await _userService.GetUserFromHeader(Authorization);

            if (user is null)
            {
                return Unauthorized();
            }
            else if (user.Type != AccountType.Admin)
            {
                return Forbid();
            }

            var account = await _firestore.GetDocumentAsync<FirestoreUser>("users", id);

            if (account == null)
            {
                Console.WriteLine($"Account not found: {id}");
                return NotFound(new { error = "Account not found" });
            }

            Console.WriteLine($"Account found: {account.Name}");
            UserDTO accountDTO = new()
            {
                Id = id,
                Type = account.Type,
                Email = account.Email,
                Banned = account.Banned,
                BanMsg = account.BanMsg,
                Name = account.Name,
                ReviewedBy = account.ReviewedBy,
            };

            return new DataResponse<UserDTO>()
            {
                Success = true,
                Data = accountDTO
            };
        }

        [HttpGet("vendors")]
        public async Task<ActionResult<ListResponse<UserDTO>>> GetAllVendors([FromHeader] string? Authorization)
        {
            var apiKey = _configuration["Firebase:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                return StatusCode(500, new ListResponse<UserDTO>
                {
                    Success = false,
                    Message = "Firebase configuration error"
                });
            }

            // Make sure admin
            var user = await _userService.GetUserFromHeader(Authorization);

            if (user is null)
            {
                return Unauthorized();
            }
            else if (user.Type != AccountType.Admin)
            {
                return Forbid();
            }

            var accounts = await _firestore.QueryComplexDocumentsAsyncWithId<Models.Firestore.User>(
                "users",
                [
                    new() {
                    fieldName = "Type",
                    value = (int)AccountType.Vendor
                }, new () {
                    fieldName = "Banned",
                    value = false
                }
                ]
            );

            List<UserDTO> accountDTOs = [];

            foreach (var account in accounts)
            {
                UserDTO accountDTO = new()
                {
                    Id = account.Id,
                    Type = account.Type,
                    Email = account.Email,
                    Banned = account.Banned,
                    BanMsg = account.BanMsg,
                    Name = account.Name,
                    ReviewedBy = account.ReviewedBy,
                    BalanceInCents = account.BalanceInCents
                };
                accountDTOs.Add(accountDTO);
            }

            return new ListResponse<UserDTO>
            {
                Success = true,
                List = accountDTOs
            };
        }

        [HttpGet("customers")]
        public async Task<ActionResult<ListResponse<UserDTO>>> GetAllCustomers([FromHeader] string? Authorization)
        {
            var apiKey = _configuration["Firebase:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                return StatusCode(500, new ListResponse<UserDTO>
                {
                    Success = false,
                    Message = "Firebase configuration error"
                });
            }

            // Make sure admin
            var user = await _userService.GetUserFromHeader(Authorization);

            if (user is null)
            {
                return Unauthorized();
            }
            else if (user.Type != AccountType.Admin)
            {
                return Forbid();
            }

            var accounts = await _firestore.QueryComplexDocumentsAsyncWithId<Models.Firestore.User>(
                "users",
                [
                    new() {
                    fieldName = "Type",
                    value = (int)AccountType.Customer
                }, new () {
                    fieldName = "Banned",
                    value = false
                }
                ]
            );

            List<UserDTO> accountDTOs = [];

            foreach (var account in accounts)
            {
                UserDTO accountDTO = new()
                {
                    Id = account.Id,
                    Type = account.Type,
                    Email = account.Email,
                    Banned = account.Banned,
                    BanMsg = account.BanMsg,
                };
                accountDTOs.Add(accountDTO);
            }

            return new ListResponse<UserDTO>
            {
                Success = true,
                List = accountDTOs
            };
        }

        [HttpGet("banned")]
        public async Task<ActionResult<ListResponse<UserDTO>>> GetAllBannedUsers([FromHeader] string? Authorization)
        {
            var apiKey = _configuration["Firebase:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                return StatusCode(500, new ListResponse<UserDTO>
                {
                    Success = false,
                    Message = "Firebase configuration error"
                });
            }

            // Make sure admin
            var user = await _userService.GetUserFromHeader(Authorization);

            if (user is null)
            {
                return Unauthorized();
            }
            else if (user.Type != AccountType.Admin)
            {
                return Forbid();
            }

            var accounts = await _firestore.QueryComplexDocumentsAsyncWithId<Models.Firestore.User>(
                "users",
                [
                    new () {
                    fieldName = "Banned",
                    value = true
                }
                ]
            );

            List<UserDTO> accountDTOs = [];

            foreach (var account in accounts)
            {
                UserDTO accountDTO = new()
                {
                    Id = account.Id,
                    Type = account.Type,
                    Email = account.Email,
                    Banned = account.Banned,
                    BanMsg = account.BanMsg,
                    Name = account.Name,
                    ReviewedBy = account.ReviewedBy,
                };
                accountDTOs.Add(accountDTO);
            }

            return new ListResponse<UserDTO>
            {
                Success = true,
                List = accountDTOs
            };
        }

        [HttpGet("requests")]
        public async Task<ActionResult<ListResponse<RequestDTO>>> GetVendorRequests()
        {
            try
            {
                Console.WriteLine("Fetching vendor requests");


                var requests = await _firestore.GetCollectionAsync<Models.Firestore.Request>("requests");
                Console.WriteLine($"Received request: {System.Text.Json.JsonSerializer.Serialize(requests)}");
                if (requests == null || !requests.Any())
                {
                    return Ok(new ListResponse<RequestDTO>
                    {
                        Success = true,
                        Message = "No vendor requests found",
                        List = new List<RequestDTO>()
                    });
                }

                // Map to DTOs without exposing sensitive data
                var requestDTOs = requests.Select(request => new RequestDTO
                {
                    Id = request.Id,
                    Email = request.Email,
                    Name = request.Name,
                    Password = request.Password,
                    Reason = request.Reason,
                    Timestamp = request.Timestamp,
                    Archived = request.Archived
                }).ToList();

                return Ok(new ListResponse<RequestDTO>
                {
                    Success = true,
                    Message = $"Retrieved {requestDTOs.Count} vendor request(s) successfully.",
                    List = requestDTOs
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching vendor requests: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                return StatusCode(500, new ListResponse<RequestDTO>
                {
                    Success = false,
                    Message = "An error occurred while retrieving vendor requests.",
                    List = new List<RequestDTO>()
                });
            }
        }

    }
}