using ApiContracts.Enums;
using Microsoft.Extensions.DependencyInjection;
using PersistanceContracts;
using Entities.Driver;
namespace Services.RecruitDriver;


public class RecruitDriverService : IRecruitDriverService
{
    private readonly IFleetPersistanceHandler _handler;
    
    public RecruitDriverService(
        [FromKeyedServices(HandlerType.Job)] IFleetPersistanceHandler handler)
    {
        _handler = handler;
    }

    public Task<Driver> RecruitDriverAsync(Driver driver, Dispatcher dispatcher)
    {
        throw new NotImplementedException();
    }
    public Task FireDriverAsync(Driver driver, Dispatcher dispatcher)
    {
        throw new NotImplementedException();
    }
    public IQueryable<Driver> GetDispatcherDriversListAsync(int id)
    {
        throw new NotImplementedException();
    }
    public IQueryable<Driver> GetDriverListWoDispatcherAsync()
    {
        throw new NotImplementedException();
    }
}