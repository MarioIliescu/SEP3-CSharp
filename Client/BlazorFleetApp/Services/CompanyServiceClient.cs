using System.Net.Http;
using System.Net.Http.Json;
using ApiContracts.Company;

namespace BlazorFleetApp.Services;

public class CompanyServiceClient
{
    private readonly HttpClient _http;

    public CompanyServiceClient(HttpClient http)
    {
        _http = http;
    }

    // Get all companies
    public async Task<List<CreateCompanyDto>> GetAllAsync()
    {
        return await _http.GetFromJsonAsync<List<CreateCompanyDto>>("company") ?? new List<CreateCompanyDto>();
    }

    // Create a company
    public async Task CreateAsync(CreateCompanyDto company)
    {
        var response = await _http.PostAsJsonAsync("company", company);
        response.EnsureSuccessStatusCode();
    }

    // Update a company
    public async Task UpdateAsync(CreateCompanyDto company)
    {
        var response = await _http.PutAsJsonAsync("company", company);
        response.EnsureSuccessStatusCode();
    }

    // Delete a company
    public async Task DeleteAsync(string mcNumber)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"company/{mcNumber}");
        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}