using System.ComponentModel;
using ApiContracts;
using ApiContracts.Enums;
using Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PersistanceContracts;
using Repositories;

namespace PersistanceHandlersGrpc.UserPersistance;

public class DispatcherHandlerGrpc : IFleetPersistanceHandler
{
    private readonly IDispatcherRepository _dispatcherRepository;
    private readonly ILogger<DispatcherHandlerGrpc> _logger;

    public DispatcherHandlerGrpc([FromKeyedServices(HandlerType.Dispatcher)]IDispatcherRepository dispatcherRepository
    , ILogger<DispatcherHandlerGrpc> logger)
    {
        _dispatcherRepository = dispatcherRepository;
        _logger = logger;
    }
    public Task<object> HandleAsync(Request request)
    {
        throw new NotImplementedException();
    }
    private async Task<object> HandleDispatcherAsync(ActionType actionType, Dispatcher request)
    {
        switch (actionType)
        {
            case ActionType.Create:
                _logger.LogInformation($"Creating dispatcher {request}");
                return await _dispatcherRepository.CreateAsync(request);
            case ActionType.Update:
                _logger.LogInformation($"Updating dispatcher {request}");
                await  _dispatcherRepository.UpdateAsync(request);
                _logger.LogInformation($"Updated dispatcher {request}");
                break;
            case ActionType.Delete:
                _logger.LogInformation("$Trying to delete dispatcher {request}");
                await _dispatcherRepository.DeleteAsync(request.Id);
                _logger.LogInformation($"Deleted driver with Id{request.Id}");
                break;
            case ActionType.Get:
                _logger.LogInformation($"Fetching dispatcher {request}");
                return await _dispatcherRepository.GetSingleAsync(request.Id);
            case ActionType.List:
                _logger.LogInformation($"Getting dispatchers {request}");
                return  _dispatcherRepository.GetManyAsync();
            default:
                _logger.LogError($"Unknown action type {actionType}");
                throw new InvalidEnumArgumentException("Unknown action type");
        }
        return Task.CompletedTask;
    }
}