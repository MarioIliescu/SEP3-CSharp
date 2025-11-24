using ApiContracts.Dtos.Dispatcher;
using ApiContracts.Dtos.Driver;

namespace BlazorFleetApp.Services.Dispatcher;

public class DispatcherServiceClient : IDispatcherService
{

    private readonly HttpClient _http;

    public DispatcherServiceClient(HttpClient http)
    {
        _http = http;
    }
    
    public async Task CreateAsync(CreateDispatcherDto dto)
    {
        var response = await _http
            .PostAsJsonAsync("dispatcher", dto);
        
        response.EnsureSuccessStatusCode();
    }
    
    public async Task UpdateAsync(DispatcherDto dto)
    {
        var response = await _http.PutAsJsonAsync("disaptcher", dto);
        response.EnsureSuccessStatusCode();
    }
    
    public async Task<DispatcherDto?> GetSingleAsync(int id)
    {
        return await _http.GetFromJsonAsync<DispatcherDto>($"dispatcher/{id}");
    }
    
    public async Task<List<DispatcherDto>> GetAllAsync()
    {
        return await _http.GetFromJsonAsync<List<DispatcherDto>>("dispatcher") ?? new List<DispatcherDto>();
    }
    
    public async Task DeleteAsync(int id)
    {
        var response = await _http.DeleteAsync($"dispatcher/{id}");
        response.EnsureSuccessStatusCode();
    }
}