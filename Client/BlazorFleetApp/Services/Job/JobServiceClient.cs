using ApiContracts.Dtos.Job;

namespace BlazorFleetApp.Services.Job;

public class JobServiceClient : IJobService
{
    private readonly HttpClient _http;

    public JobServiceClient(HttpClient http)
    {
        _http = http;
    }
    public async Task CreateAsync(CreateJobDto dto)
    {
        var response = await _http
            .PostAsJsonAsync("job", dto);
        
        response.EnsureSuccessStatusCode();    }

    public async Task UpdateAsync(UpdateJobDto dto)
    {
        var response = await _http.PutAsJsonAsync("job", dto);
        response.EnsureSuccessStatusCode();    }

    public async Task<JobDto?> GetSingleAsync(int id)
    {
        return await _http.GetFromJsonAsync<JobDto>($"job/{id}");
    }

    public async Task<List<JobDto>> GetAllAsync()
    {
        return await _http.GetFromJsonAsync<List<JobDto>>("job") ?? new List<JobDto>();
    }

    public async Task DeleteAsync(int id)
    {
        var response = await _http.DeleteAsync($"job/{id}");
        response.EnsureSuccessStatusCode();
    }
}