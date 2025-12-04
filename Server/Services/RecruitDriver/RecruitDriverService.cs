using ApiContracts;
using ApiContracts.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PersistanceContracts;
using Repositories;

namespace Services.RecruitDriver;
using Entities;
public class RecruitDriverService(
    ILogger<RecruitDriverService> logger,
    [FromKeyedServices(HandlerType.Recruit)] IFleetPersistanceHandler handler)
    : IRecruitDriverService
{
    
    public async Task<Driver> RecruitDriverAsync(Driver driver, Dispatcher dispatcher)
    {
        Request request = MakeRecruitDriverRequest(ActionType.Create, new RecruitDriver()
        {
            Driver = driver,
            Dispatcher = dispatcher
        });
        logger.LogInformation($"Trying to recruit driver {driver} for dispatcher {dispatcher}");
        return (Driver)await handler.HandleAsync(request);
    }

    public async Task FireDriverAsync(Driver driver, Dispatcher dispatcher)
    {
        Request request = MakeRecruitDriverRequest(
            ActionType.Delete, new RecruitDriver()
            {
                Driver = driver,
                Dispatcher = dispatcher
            }
        );
        logger.LogInformation($"Trying to fire driver {driver} for dispatcher {dispatcher}");
        await handler.HandleAsync(request);
        logger.LogInformation($"Fired driver {driver} successfully");
    }

    public IQueryable<Driver> GetDispatcherDriversListAsync(int id)
    {
        Request request = MakeRecruitDriverRequest(ActionType.Get, new RecruitDriver()
        {
            Driver = new Driver.Builder().Build(),
            Dispatcher = new Dispatcher.Builder().SetId(id).Build()
        });
        return handler.HandleAsync(request).Result  as IQueryable<Driver> ?? throw new InvalidOperationException();
    }

    public IQueryable<Driver> GetDriverListWoDispatcherAsync()
    {
        Request request = MakeRecruitDriverRequest(ActionType.List, new RecruitDriver()
        {
            Driver = new Driver.Builder().Build(),
            Dispatcher = new Dispatcher.Builder().Build()
        });
        return handler.HandleAsync(request).Result  as IQueryable<Driver> ?? throw new InvalidOperationException();
    }
    
    private Request MakeRecruitDriverRequest(ActionType action, RecruitDriver recruitDriver)
    {
        return new Request(action, HandlerType.Recruit, recruitDriver);
    }
}