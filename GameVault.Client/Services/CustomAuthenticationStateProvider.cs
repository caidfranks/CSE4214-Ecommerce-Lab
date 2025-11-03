using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace GameVault.Client.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly AuthService _authService;

        public CustomAuthenticationStateProvider(AuthService authService)
        {
            _authService = authService;
            _authService.OnAuthStateChanged += NotifyAuthStateChanged;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = _authService.CurrentUser;
            
            if (user != null && _authService.IsAuthenticated)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, _authService.CurrentUserId ?? ""),
                    new Claim(ClaimTypes.Name, user.Name ?? ""),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim(ClaimTypes.Role, user.Type.ToString())
                };

                var identity = new ClaimsIdentity(claims, "cookie");
                var principal = new ClaimsPrincipal(identity);
                
                return Task.FromResult(new AuthenticationState(principal));
            }

            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        }

        private void NotifyAuthStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}