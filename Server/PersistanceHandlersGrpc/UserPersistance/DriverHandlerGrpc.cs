using System.ComponentModel;
using ApiContracts;
using ApiContracts.Enums;
using Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PersistanceContracts;
using Repositories;

namespace PersistanceHandlersGrpc.UserPersistance;

public class DriverHandlerGrpc : IFleetPersistanceHandler
{
    private readonly IDriverRepository _driverRepository;
    private ILogger<DriverHandlerGrpc> _logger;

    public DriverHandlerGrpc([FromKeyedServices(HandlerType.Driver)] IDriverRepository driverRepository,
        ILogger<DriverHandlerGrpc> logger)
    {
        _driverRepository = driverRepository;
        _logger = logger;
    }
    public async Task<object> HandleAsync(Request request)
    {
        Driver driver = request.Payload as Driver ?? throw new ArgumentException("Invalid request payload");
        switch (request.Action)
        {
            case ActionType.Create:
                _logger.LogInformation($"Creating driver {driver}");
                return await _driverRepository.CreateAsync(driver);
            case ActionType.Update:
                _logger.LogInformation($"Updating driver {driver}");
                await  _driverRepository.UpdateAsync(driver);
                _logger.LogInformation($"Updated driver {driver}");
                break;
            case ActionType.Delete:
                _logger.LogInformation($"$Trying to delete driver {driver}");
                await _driverRepository.DeleteAsync(driver.Id);
                _logger.LogInformation($"Deleted driver with Id{driver.Id}");
                break;
            case ActionType.Get:
                _logger.LogInformation($"Fetching driver {driver}");
                return await _driverRepository.GetSingleAsync(driver.Id);
            case ActionType.List:
                _logger.LogInformation($"Getting drivers {driver}");
                return  _driverRepository.GetManyAsync();
            default:
                _logger.LogError($"Unknown action type {request.Action}");
                throw new InvalidEnumArgumentException("Unknown action type");
        }
        return Task.CompletedTask;
    }
}