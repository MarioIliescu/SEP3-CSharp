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
    public async Task<object> HandleAsync(Request request)
    {
        Dispatcher dispatcher = request.Payload as Dispatcher ?? throw new ArgumentException("Invalid request payload");
        switch (request.Action)
        {
            case ActionType.Create:
                _logger.LogInformation($"Creating dispatcher {dispatcher}");
                return await _dispatcherRepository.CreateAsync(dispatcher);
            case ActionType.Update:
                _logger.LogInformation($"Updating dispatcher {dispatcher}");
                await  _dispatcherRepository.UpdateAsync(dispatcher);
                _logger.LogInformation($"Updated dispatcher {dispatcher}");
                break;
            case ActionType.Delete:
                _logger.LogInformation("$Trying to delete dispatcher {dispatcher}");
                await _dispatcherRepository.DeleteAsync(dispatcher.Id);
                _logger.LogInformation($"Deleted driver with Id{dispatcher.Id}");
                break;
            case ActionType.Get:
                _logger.LogInformation($"Fetching dispatcher {dispatcher}");
                return await _dispatcherRepository.GetSingleAsync(dispatcher.Id);
            case ActionType.List:
                _logger.LogInformation($"Getting dispatchers {dispatcher}");
                return  _dispatcherRepository.GetManyAsync();
            default:
                _logger.LogError($"Unknown action type {request.Action}");
                throw new InvalidEnumArgumentException("Unknown action type");
        }
        return Task.CompletedTask;
    }
}