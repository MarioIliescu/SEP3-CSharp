using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorFleetApp.Authentification;

public class AuthProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private ClaimsPrincipal _currentClaimsPrincipal;

    public AuthProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task Login(string email, string password)
    {
        HttpResponseMessage response =
            await _httpClient.PostAsJsonAsync("auth/login", new LoginRequest(email, password));

        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(content);
        }

        UserDto userDto = JsonSerializer.Deserialize<UserDto>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Email, userDto.Email),
            new Claim("Id", userDto.Id),
            new Claim(ClaimTypes.Role, userDto.UserRole)
        };

        ClaimsIdentity identity = new ClaimsIdentity(claims, "apiauth");
        _currentClaimsPrincipal = new ClaimsPrincipal(identity);
        
        NotifyAuthenticationStateChanged( Task.FromResult(new AuthenticationState(_currentClaimsPrincipal)));
    }

    public void Logout()
    {
        _currentClaimsPrincipal = new();
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentClaimsPrincipal)));
    }
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return new AuthenticationState(_currentClaimsPrincipal ?? new());
    }
}