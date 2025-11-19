using ApiContracts.Dtos.Driver;

namespace BlazorFleetApp.Services.Driver;

public class DriverServiceClient : IDriverService
{

    private readonly HttpClient _http;

    public DriverServiceClient(HttpClient http)
    {
        _http = http;
    }
    
    public async Task CreateAsync(CreateDriverDto dto)
    {
        var response = await _http
            .PostAsJsonAsync("driver", dto);
        
        response.EnsureSuccessStatusCode();
    }
    
    public async Task UpdateAsync(DriverDto dto)
    {
        var response = await _http.PutAsJsonAsync("driver", dto);
        response.EnsureSuccessStatusCode();
    }
    
    public async Task<DriverDto?> GetSingleAsync(int id)
    {
        return await _http.GetFromJsonAsync<DriverDto>($"driver/{id}");
    }
    
    public async Task<List<DriverDto>> GetAllAsync()
    {
        return await _http.GetFromJsonAsync<List<DriverDto>>("driver") ?? new List<DriverDto>();
    }
    
    public async Task DeleteAsync(int id)
    {
        var response = await _http.DeleteAsync($"driver/{id}");
        response.EnsureSuccessStatusCode();
    }
}