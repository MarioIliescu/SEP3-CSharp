using System.Security.Claims;
using System.Text.Json;
using ApiContracts.Dtos.Authetication;
using ApiContracts.Dtos.User;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace BlazorFleetApp.Authentification;

public class AuthProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;

    public AuthProvider(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
    }

    public async Task LoginAsync(string email, string password)
    {
        HttpResponseMessage response =
            await _httpClient.PostAsJsonAsync("auth/login", new LoginRequest{Email = email, Password = password});

        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(content);
        }

        LoginDto loginDto = JsonSerializer.Deserialize<LoginDto>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        string serializedData = JsonSerializer.Serialize(loginDto);
        await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser", serializedData);

        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Email, loginDto.Email),
            new Claim("Id", loginDto.Id.ToString()),
            new Claim(ClaimTypes.Role, loginDto.UserRole.ToString())
        };
        
        ClaimsIdentity identity = new ClaimsIdentity(claims, "apiauth");
        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);
        
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }

    public async Task Logout()
    {
        await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser", "");
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new())));
    }
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string userAsJson = "";
        try
        {
            userAsJson = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "currentUser");
        }
        catch (InvalidOperationException e)
        {
            return new AuthenticationState(new());
        }

        if (string.IsNullOrEmpty(userAsJson))
        {
            return new AuthenticationState(new());
        }

        LoginDto loginDto = JsonSerializer.Deserialize<LoginDto>(userAsJson)!;
        
        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Email, loginDto.Email),
            new Claim("Id", loginDto.Id.ToString()),
            new Claim(ClaimTypes.Role, loginDto.UserRole.ToString())
        };
        
        ClaimsIdentity identity = new ClaimsIdentity(claims, "apiauth");
        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);
        return new AuthenticationState(claimsPrincipal);
    }
}