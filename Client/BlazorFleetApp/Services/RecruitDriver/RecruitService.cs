using ApiContracts.Dtos.Driver;
using ApiContracts.Dtos.RecruitDriver;
using BlazorFleetApp.Services.Events;
using Microsoft.AspNetCore.SignalR;

namespace BlazorFleetApp.Services.RecruitDriver;

public class RecruitService(HttpClient http) : IRecruitService
{
    private readonly HttpClient _http = http;
    public async Task RecruitDriver(RecruitDriverDto dto)
    {
        var response = await _http.PostAsJsonAsync("recruit-driver", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task FireDriver(RecruitDriverDto dto)
    {
        var response = await _http.DeleteAsync($"recruit-driver/delete");
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<DriverDto>> GetDriversForDispatcher(int dispatcherId)
    {
        return await _http.GetFromJsonAsync<List<DriverDto>>($"recruit-driver/dispatcher/{dispatcherId}") ?? new List<DriverDto>();
    }

    public async Task<List<DriverDto>> GetDrivers()
    {
        return await _http.GetFromJsonAsync<List<DriverDto>>($"recruit-driver/unassigned") ?? new List<DriverDto>();
    }
}