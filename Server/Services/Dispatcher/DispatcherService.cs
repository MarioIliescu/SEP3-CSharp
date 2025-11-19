namespace Services.Dispatcher;

using ApiContracts;
using ApiContracts.Enums;
using PersistanceContracts;
using Entities;
using Microsoft.Extensions.DependencyInjection;

public class DispatcherService : IDispatcherService
{
    private readonly IFleetPersistanceHandler _handler;
    
    public DispatcherService([FromKeyedServices (HandlerType.Dispatcher)] IFleetPersistanceHandler handler)
    {
        this._handler = handler;
    }

    public async Task<Dispatcher> CreateAsync(Dispatcher payload)
    {
        Request request = MakeDispatcherRequest(ActionType.Create, payload);
        return (Dispatcher) await  _handler.HandleAsync(request);
    }
    
    public async Task UpdateAsync(Dispatcher payload)
    {
        Request request = MakeDispatcherRequest(ActionType.Update, payload);
        await  _handler.HandleAsync(request);
    }
    
    public async Task<Dispatcher> GetSingleAsync(int id)
    {
        Request request = MakeDispatcherRequest(
            ActionType.Get, new Dispatcher()
            {
                Id = id
            }
        );
        return (Dispatcher) await _handler.HandleAsync(request);
    }
    
    public async Task DeleteAsync(int id)
    {
        Request request = MakeDispatcherRequest(
            ActionType.Delete, new Dispatcher()
            {
                Id = id
            }
        );
        await _handler.HandleAsync(request);
    }
    
    public IQueryable<Dispatcher> GetManyAsync()
    {
        Request request = MakeDispatcherRequest(ActionType.List, new Dispatcher.Builder()
            .Build());

        var result = _handler.HandleAsync(request).Result as IQueryable<Dispatcher> ;
        if (result != null)
        {
            return result;
        }
        throw new InvalidOperationException("No drivers found");
    }
    
    private Request MakeDispatcherRequest(ActionType action, Dispatcher dispatcher)
    {
        return new Request(action, HandlerType.Dispatcher, dispatcher);
    }
}
