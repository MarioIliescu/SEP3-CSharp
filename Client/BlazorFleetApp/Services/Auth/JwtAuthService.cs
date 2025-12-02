using System.Security.Claims;
using System.Text;
using System.Text.Json;
using ApiContracts.Dtos.Auth;
using ApiContracts.Dtos.Authetication;
using ApiContracts.Enums;
using Entities;
using Microsoft.JSInterop;

namespace BlazorFleetApp.Services.Auth;

public class JwtAuthService(HttpClient client, IJSRuntime jsRuntime) : IAuthService
{
    // this private variable for simple caching
    public string Jwt { get; private set; } = "";

    public Action<ClaimsPrincipal> OnAuthStateChanged { get; set; } = null!;

    public async Task LoginAsync(string email, string password)
    {
        LoginRequest userLoginDto = new()
        {
            Email = email,
            Password = password
        };

        string userAsJson = JsonSerializer.Serialize(userLoginDto);
        StringContent content = new(userAsJson, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PostAsync("/auth/login", content);
        string responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(responseContent);
        }

        var json = JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent);
        if (json == null || !json.TryGetValue("token", out string token))
            throw new Exception("Invalid login response: missing token");

        Jwt = token;

        await CacheTokenAsync();

        ClaimsPrincipal principal = await CreateClaimsPrincipal();

        OnAuthStateChanged.Invoke(principal);
    }

    private async Task<ClaimsPrincipal> CreateClaimsPrincipal()
    {
        if (string.IsNullOrEmpty(Jwt))
        {
            var cachedToken = await GetTokenFromCacheAsync();
            if (!string.IsNullOrEmpty(cachedToken))
                Jwt = cachedToken;
        }
        if (string.IsNullOrEmpty(Jwt))
            return new ClaimsPrincipal();
        client.DefaultRequestHeaders.Remove("Authorization");
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Jwt);
        IEnumerable<Claim> claims = ParseClaimsFromJwt(Jwt);
        ClaimsIdentity identity = new(claims, "jwt");

        return new ClaimsPrincipal(identity);
    }

    public async Task LogoutAsync()
    {
        await ClearTokenFromCacheAsync();
        client.DefaultRequestHeaders.Remove("Authorization");
        Jwt = "";
        ClaimsPrincipal principal = new();
        OnAuthStateChanged.Invoke(principal);
    }

    public async Task RegisterAsync(User user)
    {
        string userAsJson = JsonSerializer.Serialize(user, user.GetType());
        StringContent content = new(userAsJson, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await client.PostAsync("http://localhost:5124/auth/register", content);
        string responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(responseContent);
        }
    }

    public async Task<ClaimsPrincipal> GetAuthAsync()
    {
        ClaimsPrincipal principal = await CreateClaimsPrincipal();
        return principal;
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        string payload = jwt.Split('.')[1];
        byte[] jsonBytes = ParseBase64WithoutPadding(payload);
        Dictionary<string, object>? keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes)
                                                    ?? new Dictionary<string, object>();

        return keyValuePairs.Select(kvp =>
        {
            return new Claim(kvp.Key, kvp.Value.ToString()!);
        });
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2:
                base64 += "==";
                break;
            case 3:
                base64 += "=";
                break;
        }

        return Convert.FromBase64String(base64);
    }

    private async Task<string?> GetTokenFromCacheAsync()
    {
        return await jsRuntime.InvokeAsync<string>("localStorage.getItem", "jwt");
    }

    private async Task CacheTokenAsync()
    {
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", "jwt", Jwt);
    }

    private async Task ClearTokenFromCacheAsync()
    {
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", "jwt", "");
    }
    public async Task RefreshTokenAsync()
    {
        if (string.IsNullOrEmpty(Jwt))
        {
            var cachedToken = await GetTokenFromCacheAsync();
            if (string.IsNullOrEmpty(cachedToken))
                throw new InvalidOperationException("No JWT available to refresh.");
            Jwt = cachedToken;
        }
        client.DefaultRequestHeaders.Remove("Authorization");
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Jwt);

        HttpResponseMessage response = await client.PostAsync("/auth/refresh", null);
        string responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Token refresh failed: {responseContent}");
        }

        var json = JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent);
        if (json == null || !json.TryGetValue("token", out string token))
            throw new Exception("Invalid refresh response: missing token");

        Jwt = token;
        await CacheTokenAsync();
        client.DefaultRequestHeaders.Remove("Authorization");
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Jwt);
        ClaimsPrincipal principal = await CreateClaimsPrincipal();
        OnAuthStateChanged.Invoke(principal);
    }
}