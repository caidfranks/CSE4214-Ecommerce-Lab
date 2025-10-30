using System.Resources;
using GameVault.Server.Models;

namespace GameVault.Server.Services;

public class UserService
{
  private readonly IFirebaseAuthService _firebaseAuth;
  private readonly IFirestoreService _firestore;

  public UserService(IFirebaseAuthService firebaseAuth, IFirestoreService firestore)
  {
    _firebaseAuth = firebaseAuth;
    _firestore = firestore;
  }

  public async Task<User?> GetUserFromHeader(string? header)
  {
    if (header is null) return null;
    try
    {
      string token = header.Split(" ").ToList()[1]; // Extract 2nd part of header formatted "Bearer [___Token___]"

      var userId = await _firebaseAuth.VerifyTokenAsync(token);

      if (userId is null) return null;

      var user = await _firestore.GetDocumentAsync<User>("users", userId);

      return user;
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex);
      return null;
    }
  }
}