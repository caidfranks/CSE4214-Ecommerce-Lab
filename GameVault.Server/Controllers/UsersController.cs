using Microsoft.AspNetCore.Mvc;
using GameVault.Server.Models;
using GameVault.Server.Services;
using GameVault.Server.Models.Firestore;

namespace GameVault.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IFirestoreService _firestore;
    private readonly IFirebaseAuthService _firebaseAuth;

    public UsersController(IFirestoreService firestore, IFirebaseAuthService firebaseAuth)
    {
        _firestore = firestore;
        _firebaseAuth = firebaseAuth;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<Models.Firestore.User>> GetUser(string userId)
    {
        var user = await _firestore.GetDocumentAsync<Models.Firestore.FirestoreUser>("users", userId);

        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        return Ok(user);
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
}

public class UpdateUserRequest
{
    public string? DisplayName { get; set; }
}