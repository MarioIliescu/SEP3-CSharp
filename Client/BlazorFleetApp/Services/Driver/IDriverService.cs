using ApiContracts.Company;
using ApiContracts.Dtos.Driver;

namespace BlazorFleetApp.Services.Driver;

public interface IDriverService
{
    public Task CreateAsync(CreateDriverDto dto);
    public Task UpdateAsync(DriverDto dto);
    public Task<DriverDto?> GetSingleAsync(int id);
    public Task<List<DriverDto>> GetAllAsync();
    public Task DeleteAsync(int id);
}