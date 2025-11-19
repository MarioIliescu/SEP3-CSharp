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
        switch (request.Handler)
        {
            case HandlerType.Driver:
                Driver payload = request.Payload as Driver;
                _logger.LogInformation($"Handling driver {payload}");
                return await HandleDriverAsync(request.Action, payload);
            default:
                _logger.LogError($"Unhandled handler type {request.Handler}");
                throw new NotImplementedException();
        }
    }

    private async Task<object> HandleDriverAsync(ActionType actionType, Driver request)
    {
        switch (actionType)
        {
            case ActionType.Create:
                _logger.LogInformation($"Creating driver {request}");
                return await _driverRepository.CreateAsync(request);
            case ActionType.Update:
                _logger.LogInformation($"Updating driver {request}");
                await  _driverRepository.UpdateAsync(request);
                _logger.LogInformation($"Updated driver {request}");
                break;
            case ActionType.Delete:
                _logger.LogInformation("$Trying to delete driver {request}");
                await _driverRepository.DeleteAsync(request.Id);
                _logger.LogInformation($"Deleted driver with Id{request.Id}");
                break;
            case ActionType.Get:
                _logger.LogInformation($"Fetching driver {request}");
                return await _driverRepository.GetSingleAsync(request.Id);
            case ActionType.List:
                _logger.LogInformation($"Getting drivers {request}");
                return  _driverRepository.GetManyAsync();
            default:
                _logger.LogError($"Unknown action type {actionType}");
                throw new InvalidEnumArgumentException("Unknown action type");
        }
        return Task.CompletedTask;
    }
    
}