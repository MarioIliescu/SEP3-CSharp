using ApiContracts.Company;
using ApiContracts.Dtos.Dispatcher;

namespace BlazorFleetApp.Services.Dispatcher;

public interface IDispatcherService
{
    public Task CreateAsync(CreateDispatcherDto dto);
    public Task UpdateAsync(DispatcherDto dto);
    public Task<DispatcherDto?> GetSingleAsync(int id);
    public Task<List<DispatcherDto>> GetAllAsync();
    public Task DeleteAsync(int id);
}