server-side usage:

using GameVault.Server.Filters;
using GameVault.Server.Services;

[Authorize]
[RequireAuthUser]

public class XXXXXXX
{
    private readonly ICurrentUserService _currentUser;

    in constructor, take in "ICurrentUserService currentUser" as a parameter and define "_currentUser = currentUser;" within constructor


}

in class, use "_currentUser.UserId"