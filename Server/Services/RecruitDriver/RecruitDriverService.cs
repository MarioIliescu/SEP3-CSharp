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
    private readonly ILogger<RecruitDriverService> _logger = logger;
    private readonly IFleetPersistanceHandler _handler = handler;

    public async Task<Driver> RecruitDriverAsync(Driver driver, Dispatcher dispatcher)
    {
        throw new NotImplementedException();
    }

    public async Task FireDriverAsync(Driver driver, Dispatcher dispatcher)
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