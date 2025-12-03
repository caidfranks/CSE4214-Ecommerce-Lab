using Microsoft.AspNetCore.Mvc;

namespace GameVault.Server.Filters;

public class RequireAuthUser : TypeFilterAttribute
{
    public RequireAuthUser() :
    base(typeof(RequireAuthUser))
    {

    }
}
