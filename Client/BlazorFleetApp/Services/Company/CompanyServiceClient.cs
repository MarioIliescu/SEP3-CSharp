using System.Net.Http;
using System.Net.Http.Json;
using ApiContracts.Company;

namespace BlazorFleetApp.Services;

public class CompanyServiceClient : ICompanyService
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

    // Create company
    public async Task CreateAsync(
        CreateCompanyDto dto)
    {
        // POST/api/company
        var response = await _http
            .PostAsJsonAsync("company", dto);

        // Optional: capture the returned 201 Location header
        // var location = response.Headers.Location;

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
    
    // Get a company
    public async Task GetSingleAsync(string mcNumber)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"company/{mcNumber}");
        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}