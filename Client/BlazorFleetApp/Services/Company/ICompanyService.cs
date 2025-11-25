using ApiContracts.Company;

namespace BlazorFleetApp.Services;

public interface ICompanyService
{
    public Task CreateAsync(CreateCompanyDto dto);
    public Task UpdateAsync(CreateCompanyDto dto);
    public Task GetSingleAsync(string mcNumber);
    public Task<List<CompanyDto>> GetAllAsync();
    public Task DeleteAsync(string mcNumber);
}