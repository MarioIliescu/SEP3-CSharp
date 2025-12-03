using System.Security.Claims;
using BlazorFleetApp.Services.Auth;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorFleetApp.Authentication;

public class AuthProvider : AuthenticationStateProvider
{
    
    private readonly IAuthService authService;

    public AuthProvider(IAuthService authService)
    {
        this.authService = authService;
        authService.OnAuthStateChanged += AuthStateChanged;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        ClaimsPrincipal principal = await authService.GetAuthAsync();

        return new AuthenticationState(principal);
    }

    private void AuthStateChanged(ClaimsPrincipal principal)
    {
        NotifyAuthenticationStateChanged(
            Task.FromResult(
                new AuthenticationState(principal)
            )
        );
    }

}
