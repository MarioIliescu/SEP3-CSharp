using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using ApiContracts.Company;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BlazorFleetApp.Services;

public class CompanyServiceClient : ICompanyService
{
    private readonly HttpClient _http;

    public CompanyServiceClient(HttpClient http)
    {
        _http = http;
    }

    // Get all companies
    public async Task<List<CompanyDto>> GetAllAsync()
    {
        return await _http.GetFromJsonAsync<List<CompanyDto>>("company") ?? new List<CompanyDto>();
    }

    // Create company
    public async Task CreateAsync(
        CreateCompanyDto dto)
    {
        // POST/api/company
        var response = await _http
            .PostAsJsonAsync("company", dto);
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception(error);
        }
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
    public async Task<CompanyDto> GetSingleAsync(string mcNumber)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"company/{mcNumber}");
        var response = await _http.SendAsync(request);
        return await response.Content.ReadFromJsonAsync<CompanyDto>()
               ?? throw new InvalidOperationException("Company not found");
    }
}