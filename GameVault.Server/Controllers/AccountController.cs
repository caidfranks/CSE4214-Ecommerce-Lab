using GameVault.Server.Models;
using GameVault.Server.Models.Firestore;
using GameVault.Server.Services;
using GameVault.Shared.DTOs;
using GameVault.Shared.Models;
using Google.Cloud.Firestore.V1;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;

namespace GameVault.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IFirestoreService _firestore;
        private readonly IConfiguration _configuration;

        public AccountController(IFirestoreService firestore, IConfiguration configuration)
        {
            _firestore = firestore;
            _configuration = configuration;
        }

        [HttpPost("approve")]
        public async Task<ActionResult<BaseResponse>> ChangeAccountStatusToActive([FromBody] string id)
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

            await _firestore.SetDocumentFieldAsync("users", id, "ApprovalStatus", (int)AccountStatus.ActiveVendor);

            // TODO: Handle firestore errors

            return Ok(new BaseResponse
            {
                Success = true,
                Message = "Listing status successfully updated to pending",
            });
        }

        [HttpPost("deny")]
        public async Task<ActionResult<BaseResponse>> ChangeAccountStatusToDenied([FromBody] string id)
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

            await _firestore.SetDocumentFieldAsync("users", id, "ApprovalStatus", (int)AccountStatus.Denied);

            // TODO: Handle firestore errors

            return Ok(new BaseResponse
            {
                Success = true,
                Message = "Listing status successfully updated to pending",
            });
        }

        [HttpPost("ban")]
        public async Task<ActionResult<BaseResponse>> ChangeAccountStatusToBanned([FromBody] string id)
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

            await _firestore.SetDocumentFieldAsync("users", id, "ApprovalStatus", (int)AccountStatus.Banned);

            // TODO: Handle firestore errors

            return Ok(new BaseResponse
            {
                Success = true,
                Message = "Listing status successfully updated to pending",
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AccountDTO>> GetAccountById(string id)
        {
            try
            {
                Console.WriteLine($"Fetching product with ID: {id}");
                var account = await _firestore.GetDocumentAsync<Account>("users", id);

                if (account == null)
                {
                    Console.WriteLine($"Account not found: {id}");
                    return NotFound(new { error = "Account not found" });
                }

                Console.WriteLine($"Account found: {account.DisplayName}");
                var accountDTO = new AccountDTO
                {
                    ApprovalStatus = account.ApprovalStatus,
                    //ApprovedAt = account.ApprovedAt?.ToDateTime().ToString("o") ?? string.Empty,
                    ApprovedBy = account.ApprovedBy,
                    BusinessName = account.BusinessName,
                    //CreatedAt = account.CreatedAt?.ToDateTime().ToString("o") ?? string.Empty,
                    DisplayName = account.DisplayName,
                    Email = account.Email,
                    RejectionReason = account.RejectionReason,
                    Role = account.Role,
                    //UpdatedAt = account.UpdatedAt?.ToDateTime().ToString("o") ?? string.Empty,
                    UserID = account.Id
                };

                return accountDTO;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("status")]
        public async Task<ActionResult<AccountListResponse>> GetAccountsByStatus([FromQuery] AccountStatus s)
        {
            var apiKey = _configuration["Firebase:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                return StatusCode(500, new AccountListResponse
                {
                    Success = false,
                    Message = "Firebase configuration error"
                });
            }

            var accounts = await _firestore.QueryComplexDocumentsAsyncWithId<Models.Firestore.Account>(
                "users",
                [
                    new() {
                fieldName = "ApprovalStatus",
                value = (int)s
            }
                ]
            );

            List<AccountDTO> accountDTOs = [];

            foreach (var account in accounts)
            {
                AccountDTO accountDTO = new()
                {
                    ApprovalStatus = account.ApprovalStatus,
                    //ApprovedAt = account.ApprovedAt,
                    ApprovedBy = account.ApprovedBy,
                    BusinessName = account.BusinessName,
                    //CreatedAt = account.CreatedAt,
                    DisplayName = account.DisplayName,
                    Email = account.Email,
                    RejectionReason = account.RejectionReason,
                    Role = account.Role,
                    //UpdatedAt = account.UpdatedAt,
                    UserID = account.Id
                };
                accountDTOs.Add(accountDTO);
            }

            return new AccountListResponse
            {
                Success = true,
                Accounts = accountDTOs
            };
        }

        [HttpPost("addRemovalReason/{id}")]
        public async Task<ActionResult<BaseResponse>> AddRemovalReason(string id, [FromBody] string reason)
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

            if (string.IsNullOrWhiteSpace(reason))
            {
                return BadRequest(new BaseResponse
                {
                    Success = false,
                    Message = "Rejection reason cannot be empty"
                });
            }

            // TODO: Make sure owner
            try
            {
                await _firestore.SetDocumentFieldAsync("users", id, "RejectionReason", reason);

                // TODO: Handle firestore errors

                return Ok(new BaseResponse
                {
                    Success = true,
                    Message = "Listing status successfully updated to pending",
                });
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, new BaseResponse
                {
                    Success = false,
                    Message = "Failed to add rejection reason"
                });
            }

        }

    }
}