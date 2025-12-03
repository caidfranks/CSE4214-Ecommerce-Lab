using GameVault.Server.Models.Firestore;
using GameVault.Server.Services;
using GameVault.Shared.Models;
using Moq;
using Xunit;

namespace GameVault.Server.Tests.Unit.Services;

public class UserServiceTests
{
    private readonly Mock<IFirebaseAuthService> _mockFirebaseAuth;
    private readonly Mock<IFirestoreService> _mockFirestore;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockFirebaseAuth = new Mock<IFirebaseAuthService>();
        _mockFirestore = new Mock<IFirestoreService>();
        _userService = new UserService(_mockFirebaseAuth.Object, _mockFirestore.Object);
    }

    [Fact]
    public async Task GetUserFromHeader_WithNullHeader_ReturnsNull()
    {
        var result = await _userService.GetUserFromHeader(null);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserFromHeader_WithValidHeader_ReturnsUser()
    {
        var token = "valid-firebase-token";
        var header = $"Bearer {token}";
        var userId = "user123";
        var firestoreUser = new FirestoreUser
        {
            Type = AccountType.Customer,
            Email = "test@example.com",
            Banned = false,
            BanMsg = null,
            Name = "Test User",
            ReviewedBy = null
        };

        _mockFirebaseAuth
            .Setup(x => x.VerifyTokenAsync(token))
            .ReturnsAsync(userId);

        _mockFirestore
            .Setup(x => x.GetDocumentAsync<FirestoreUser>("users", userId))
            .ReturnsAsync(firestoreUser);

        var result = await _userService.GetUserFromHeader(header);

        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal("test@example.com", result.Email);
        Assert.Equal(AccountType.Customer, result.Type);
    }

    [Fact]
    public async Task GetUserFromHeader_WithInvalidToken_ReturnsNull()
    {
        var token = "invalid-token";
        var header = $"Bearer {token}";

        _mockFirebaseAuth
            .Setup(x => x.VerifyTokenAsync(token))
            .ReturnsAsync((string?)null);

        var result = await _userService.GetUserFromHeader(header);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserFromHeader_WhenUserNotInFirestore_ReturnsNull()
    {
        var token = "valid-token";
        var header = $"Bearer {token}";
        var userId = "user123";

        _mockFirebaseAuth
            .Setup(x => x.VerifyTokenAsync(token))
            .ReturnsAsync(userId);

        _mockFirestore
            .Setup(x => x.GetDocumentAsync<FirestoreUser>("users", userId))
            .ReturnsAsync((FirestoreUser?)null);

        var result = await _userService.GetUserFromHeader(header);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserFromHeader_WithMalformedHeader_ReturnsNull()
    {
        var header = "InvalidHeaderFormat";

        var result = await _userService.GetUserFromHeader(header);

        Assert.Null(result);
    }
}
