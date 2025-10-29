using GameVault.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GameVault.Server.Filters;

public class RequireAuthUserFilter : IAsyncActionFilter
{
    private readonly ICurrentUserService _currentUser;

    public RequireAuthUserFilter(ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context, ActionExecutionDelegate next
    )
    {
        if (!_currentUser.IsAuthenticated || string.IsNullOrEmpty(_currentUser.UserId))
        {
            context.Result = new UnauthorizedObjectResult("user not authenticated");
            return;
        }

        await next();
    }
}