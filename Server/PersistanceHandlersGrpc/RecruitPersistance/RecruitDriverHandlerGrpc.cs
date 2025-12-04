using System.ComponentModel;
using ApiContracts;
using ApiContracts.Enums;
using Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PersistanceContracts;
using Repositories;

namespace PersistanceHandlersGrpc.RecruitPersistance;

public class RecruitDriverHandlerGrpc : IFleetPersistanceHandler
{
    private readonly IRecruitDriverRepository _recruitDriverService;
    private readonly ILogger<RecruitDriverHandlerGrpc> _logger;

    public RecruitDriverHandlerGrpc(
        [FromKeyedServices(HandlerType.Recruit)] IRecruitDriverRepository recruitDriverService,
        ILogger<RecruitDriverHandlerGrpc> logger)
    {
        _recruitDriverService = recruitDriverService;
        _logger = logger;
    }

    public async Task<object> HandleAsync(Request request)
    {
        try
        {
            var recruitDriver = (RecruitDriver)request.Payload ??
                                throw new ArgumentNullException(nameof(request.Payload));

            switch (request.Action)
            {
                case ActionType.Create:
                {
                    _logger.LogInformation(
                        $"Recruted driver {recruitDriver.Driver.Id} for dispatcher {recruitDriver.Dispatcher.Id}");
                    return await _recruitDriverService.RecruitDriverAsync(recruitDriver.Driver,
                        recruitDriver.Dispatcher);
                }

                case ActionType.Delete:
                {
                    _logger.LogInformation(
                        $"Fired driver {recruitDriver.Driver.Id} from dispatcher {recruitDriver.Dispatcher.Id}");
                    await _recruitDriverService.FireDriverAsync(recruitDriver.Driver, recruitDriver.Dispatcher);
                    break;
                }

                case ActionType.Get:
                {
                    _logger.LogInformation(
                        $"Successfully returned drivers for dispatcher [Dispatcher = {recruitDriver.Dispatcher.Id}]");
                    return _recruitDriverService.GetDispatcherDriversListAsync(recruitDriver.Dispatcher.Id);
                }

                case ActionType.List:
                {
                    _logger.LogInformation($"Successfully returned all drivers without a dispatcher");
                    return _recruitDriverService.GetDriverListWoDispatcherAsync();
                }

                default:
                {
                    _logger.LogError($"Unknown action {request.Action}");
                    throw new InvalidEnumArgumentException("Unknown action type");
                }
            }

            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            _logger.LogInformation(e.Message);
            throw new Exception(e.Message);
        }
    }
}