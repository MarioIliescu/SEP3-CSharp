using Entities;
using Repositories;

namespace InMemoryRepository;

public class CompanyInMemoryRepository : ICompanyRepository
{
    public List<Company> companies = new();
    
    public Task<Company> CreateAsync(Company payload)
    {
        payload.Id = companies.Any()
            ? companies.Max(c => c.Id) + 1
            : 1;
        companies.Add(payload);
        return Task.FromResult(payload);
    }

    public Task UpdateAsync(Company company)
    {
        Company? Company = companies.SingleOrDefault(c => c.Id == company.Id); // Finds exactly one item matching a condition. If none is found, it returns null
        if (Company is null)
        {
            throw new InvalidOperationException($"Company with ID '{company.Id}' not found");
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

    public Task DeleteAsync(int id)
    {
        Company? companyToRemove = companies.SingleOrDefault(c => c.Id == id);
        if (companyToRemove is null)
        {
            throw new InvalidOperationException($"Company with ID '{id}' not found");
        }

        companies.Remove(companyToRemove);
        return Task.CompletedTask;
    }

    public Task<Company> GetSingleAsync(int id)
    {
        Company? companyToGet = companies.SingleOrDefault(c => c.Id == id);
        if (companyToGet is null)
        {
            throw new InvalidOperationException($"Company with ID '{id}' not found");
        }
        return Task.FromResult(companyToGet);
    }

    public IQueryable<Company> GetManyAsync()
    {
        return companies.AsQueryable();
    }
}