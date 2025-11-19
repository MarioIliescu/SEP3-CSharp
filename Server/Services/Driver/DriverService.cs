using ApiContracts;

namespace Services.Driver;

using ApiContracts.Enums;
using Microsoft.Extensions.DependencyInjection;
using PersistanceContracts;
using Entities;

public class DriverService : IDriverService
{
    private readonly IFleetPersistanceHandler _handler;
    
    public DriverService([FromKeyedServices (HandlerType.Driver)] IFleetPersistanceHandler handler)
    {
        this._handler = handler;
    }

    public async Task<Driver> CreateAsync(Driver payload)
    {
        Request request = MakeDriverRequest(ActionType.Create, payload);
        return (Driver) await  _handler.HandleAsync(request);
    }
    
    public async Task UpdateAsync(Driver payload)
    {
        Request request = MakeDriverRequest(ActionType.Update, payload);
        await  _handler.HandleAsync(request);
    }
    
    public async Task<Driver> GetSingleAsync(int id)
    {
        Request request = MakeDriverRequest(
            ActionType.Get, new Driver()
            {
                Id = id
            }
        );
        return (Driver) await _handler.HandleAsync(request);
    }
    
    public async Task DeleteAsync(int id)
    {
        Request request = MakeDriverRequest(
            ActionType.Delete, new Driver()
            {
                Id = id
            }
        );
        await _handler.HandleAsync(request);
    }
    
    public IQueryable<Driver> GetManyAsync()
    {
        Request request = MakeDriverRequest(ActionType.List, new Driver.Builder()
            .Build());

        var result = _handler.HandleAsync(request).Result as IQueryable<Driver> ;
        if (result != null)
        {
            return result;
        }
        throw new InvalidOperationException("No drivers found");
    }
    
    private Request MakeDriverRequest(ActionType action, Driver driver)
    {
        return new Request(action, HandlerType.Driver, driver);
    }
}