using ApiContracts.Dtos.Driver;
using ApiContracts.Dtos.RecruitDriver;

namespace BlazorFleetApp.Services.RecruitDriver;

public class RecruitService(HttpClient http) : IRecruitService
{
    public async Task RecruitDriver(RecruitDriverDto dto)
    {
        var response = await http.PostAsJsonAsync("recruit-driver", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task FireDriver(RecruitDriverDto dto)
    {
        var response = await http.DeleteAsync($"recruit-driver/delete");
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<DriverDto>> GetDriversForDispatcher(int dispatcherId)
    {
        return await http.GetFromJsonAsync<List<DriverDto>>($"recruit-driver/dispatcher/{dispatcherId}") ?? new List<DriverDto>();
    }

    public async Task<List<DriverDto>> GetDrivers()
    {
        return await http.GetFromJsonAsync<List<DriverDto>>($"recruit-driver/unassigned") ?? new List<DriverDto>();
    }
}