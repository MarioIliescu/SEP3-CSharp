using ApiContracts.Dtos.Job;

namespace BlazorFleetApp.Services.Job;

public interface IJobService
{
    public Task CreateAsync(CreateJobDto dto);
    public Task UpdateAsync(JobDto dto);
    public Task<JobDto?> GetSingleAsync(int id);
    public Task<List<JobDto>> GetAllAsync();
    public Task DeleteAsync(int id);   
}