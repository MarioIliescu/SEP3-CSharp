using Entities;
using Repositories;

namespace InMemoryRepository;

public class CompanyInMemoryRepository : ICompanyRepository
{
    public List<Company> companies = new();
    
    public Task<Company> CreateAsync(Company payload)
    {
        companies.Add(payload);
        return Task.FromResult(payload);
    }

    public Task UpdateAsync(Company company)
    {
        Company? Company = companies.SingleOrDefault(c => c.McNumber== company.McNumber); // Finds exactly one item matching a condition. If none is found, it returns null
        if (Company is null)
        {
            throw new InvalidOperationException($"Company with ID '{company.McNumber}' not found");
        }

        companies.Remove(Company);
        companies.Add(company);
        return Task.CompletedTask;
    }

    public Task<Company> GetSingleAsync(string mcNumber)
    {
        Company? companyToGet = companies.SingleOrDefault(c => c.McNumber.Equals(mcNumber));
        if (companyToGet is null)
        {
            throw new InvalidOperationException($"Company with mcNumber '{mcNumber}' not found");
        }
        return Task.FromResult(companyToGet);
    }

    public Task DeleteAsync(string mcNumber)
    {
        Company? companyToRemove = companies.SingleOrDefault(c => c.McNumber.Equals(mcNumber));
        if (companyToRemove is null)
        {
            throw new InvalidOperationException($"Company with mcNumber '{mcNumber}' not found");
        }

        companies.Remove(companyToRemove);
        return Task.CompletedTask;
    }

    public IQueryable<Company> GetManyAsync()
    {
        return companies.AsQueryable();
    }
}