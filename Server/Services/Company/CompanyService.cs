namespace Services.Company;
using ApiContracts;
using ApiContracts.Enums;
using PersistanceContracts;
using Entities;
public class CompanyService :ICompanyService
{
    private readonly IFleetPersistanceHandler _handler;

    public CompanyService(IFleetPersistanceHandler handler)
    {
        this._handler = handler;
    }
    public async Task<Company> CreateAsync(Company payload)
    {
        //Logic needed to verify
        Request request = MakeCompanyRequest(ActionType.Create, payload);
        return (Company) await  _handler.HandleAsync(request);
    }

    public async Task UpdateAsync(Company payload)
    {
        //Logic needed to verify
        Request request = MakeCompanyRequest(ActionType.Update, payload);
        await  _handler.HandleAsync(request);
    }

    public async Task<Company> GetSingleAsync(string mcNumber)
    {
        //Logic needed to verify
        Request request = MakeCompanyRequest(ActionType.Get, 
            new Company.Builder()
            .SetMcNumber(mcNumber)
            .Build());
        return (Company) await _handler.HandleAsync(request);
    }

    public async Task<Company> GetSingleAsync(int id)
    {
        //Logic needed to verify
        Request request = MakeCompanyRequest(ActionType.Get, 
            new Company.Builder()
                .SetId(id)
                .Build());
        return (Company) await _handler.HandleAsync(request);
    }

    public async Task DeleteAsync(string mcNumber)
    {
        Request request = MakeCompanyRequest(ActionType.Delete, 
            new Company.Builder()
                .SetMcNumber(mcNumber)
                .Build());
         await _handler.HandleAsync(request);
    }

    public async Task DeleteAsync(int id)
    {
        Request request = MakeCompanyRequest(ActionType.Delete, 
            new Company.Builder()
                .SetId(id)
                .Build());
        await _handler.HandleAsync(request);
    }

    public IQueryable<Company> GetManyAsync()
    {
        Request request = MakeCompanyRequest(ActionType.List, new Company.Builder()
            .SetId(0)
            .Build());

        var result = _handler.HandleAsync(request).Result as IQueryable<Company> ;
        if (result != null)
        {
            return result;
        }
        throw new InvalidOperationException("No companies found");
    }

    private Request MakeCompanyRequest(ActionType action, Company company)
    {
        return new Request(action, HandlerType.Company, company);
    }
}