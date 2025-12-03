using Microsoft.AspNetCore.Mvc;
using GameVault.Server.Models;
using GameVault.Server.Services;
using GameVault.Server.Models.Firestore;
using GameVault.Shared.DTOs;

namespace GameVault.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IFirestoreService _firestore;
    private readonly IFirebaseAuthService _firebaseAuth;
    private readonly UserService _userService;

    public UsersController(IFirestoreService firestore, IFirebaseAuthService firebaseAuth, UserService userService)
    {
        _firestore = firestore;
        _firebaseAuth = firebaseAuth;
        _userService = userService;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<UserDTO>> GetUser(string userId, [FromHeader] string? Authorization)
    {
        var user = await _userService.GetUserFromHeader(Authorization);
        if (user == null) return Unauthorized();
        if (user.Id != userId) return Forbid();

        var firestoreUser = await _firestore.GetDocumentAsync<User>("users", userId);
        if (firestoreUser == null) return NotFound();

        return Ok(new UserDTO
        {
            Id = firestoreUser.Id,
            Email = firestoreUser.Email,
            Name = firestoreUser.Name,
            Type = firestoreUser.Type,
            Banned = firestoreUser.Banned,
            BalanceInCents = firestoreUser.BalanceInCents,
            BankRouting = firestoreUser.BankRouting,
            BankAccount = firestoreUser.BankAccount
        });
    }

    [HttpPut("{userId}")]
    public async Task<ActionResult> UpdateUser(string userId, [FromBody] UpdateUserRequest request)
    {
        var user = await _firestore.GetDocumentAsync<FirestoreUser>("users", userId);

        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        if (!string.IsNullOrEmpty(request.DisplayName))
            user.Name = request.DisplayName;

        await _firestore.SetDocumentAsync("users", userId, user);

        return Ok(new { message = "Profile updated successfully", user });
    }

    [HttpPost("vendors/approve")]
    public async Task<ActionResult> ApproveVendor([FromBody] VendorApprovalRequest request)
    {
        var user = await _firestore.GetDocumentAsync<FirestoreUser>("users", request.VendorUserId);

        if (user == null)
        {
            return NotFound(new { message = "Vendor not found" });
        }

        if (user.Type != Shared.Models.AccountType.Vendor)
        {
            return BadRequest(new { message = "User is not a vendor" });
        }

        // if (request.Approved)
        // {
        //     user.ApprovalStatus = nameof(ApprovalStatus.Approved);
        //     user.RejectionReason = null;
        // }
        // else
        // {
        //     user.ApprovalStatus = nameof(ApprovalStatus.Rejected);
        //     user.RejectionReason = request.RejectionReason;
        // }

        await _firestore.SetDocumentAsync("users", request.VendorUserId, user);

        return Ok(new
        {
            message = request.Approved ? "Vendor approved successfully" : "Vendor rejected",
            user
        });
    }

    [HttpGet("{userId}/vendor-status")]
    public async Task<ActionResult<VendorApplicationResponse>> GetVendorStatus(string userId)
    {
        var user = await _firestore.GetDocumentAsync<FirestoreUser>("users", userId);

        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        if (user.Type != Shared.Models.AccountType.Vendor)
        {
            return BadRequest(new { message = "User is not a vendor" });
        }

        var response = new VendorApplicationResponse
        {
            // Status = user.ApprovalStatus ?? "Unknown",
            // Message = user.ApprovalStatus switch
            // {
            //     nameof(ApprovalStatus.Pending) => "Your application is being reviewed by our team.",
            //     nameof(ApprovalStatus.Approved) => "Your vendor account is approved! You can now list games.",
            //     nameof(ApprovalStatus.Rejected) => $"Your application was rejected. Reason: {user.RejectionReason}",
            //     _ => "Status unknown"
            // },
            // RejectionReason = user.RejectionReason
            Status = "Unknownn",
            Message = "Unknown",
            RejectionReason = "Unknown"
        };

        return Ok(response);
    }

    [HttpPut("{userId}/bank-info")]
    public async Task<ActionResult> UpdateBankInfo(string userId, [FromBody] UpdateBankInfoRequest request, [FromHeader] string? Authorization)
    {
        var authUser = await _userService.GetUserFromHeader(Authorization);
        if (authUser == null) return Unauthorized();
        if (authUser.Id != userId) return Forbid();

        var user = await _firestore.GetDocumentAsync<FirestoreUser>("users", userId);
        if (user == null) return NotFound();

        user.BankRouting = request.BankRouting;
        user.BankAccount = request.BankAccount;

        await _firestore.SetDocumentAsync("users", userId, user);
        return Ok(new { message = "Bank info updated" });
    }

    [HttpPost("{userId}/cashout")]
    public async Task<ActionResult> Cashout(string userId, [FromBody] CashoutRequest request, [FromHeader] string? Authorization)
    {
        var authUser = await _userService.GetUserFromHeader(Authorization);
        if (authUser == null) return Unauthorized();
        if (authUser.Id != userId) return Forbid();

        var user = await _firestore.GetDocumentAsync<FirestoreUser>("users", userId);
        if (user == null) return NotFound();

        if (string.IsNullOrEmpty(user.BankRouting) || string.IsNullOrEmpty(user.BankAccount))
            return BadRequest(new { message = "Bank info not set" });

        if (request.AmountInCents <= 0)
            return BadRequest(new { message = "Invalid amount" });

        if (request.AmountInCents > user.BalanceInCents)
            return BadRequest(new { message = "Insufficient balance" });

        user.BalanceInCents -= request.AmountInCents;
        await _firestore.SetDocumentAsync("users", userId, user);

        return Ok(new { message = "Cashout successful", newBalance = user.BalanceInCents });
    }
}

public class UpdateUserRequest
{
    public string? DisplayName { get; set; }
}

public class UpdateBankInfoRequest
{
    public string? BankRouting { get; set; }
    public string? BankAccount { get; set; }
}

public class CashoutRequest
{
    public int AmountInCents { get; set; }
}