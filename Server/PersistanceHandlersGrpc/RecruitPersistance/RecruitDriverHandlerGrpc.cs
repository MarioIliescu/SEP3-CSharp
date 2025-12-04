using System.ComponentModel;
using ApiContracts;
using ApiContracts.Enums;
using Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PersistanceContracts;
using Repositories;

namespace PersistanceHandlersGrpc.RecruitPersistance;

public class RecruitDriverHandlerGrpc(
    [FromKeyedServices(HandlerType.Recruit)]
    IRecruitDriverRepository recruitDriverService,
    ILogger<RecruitDriverHandlerGrpc> logger)
    : IFleetPersistanceHandler
{
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
                    logger.LogInformation(
                        $"Recruted driver {recruitDriver.Driver.Id} for dispatcher {recruitDriver.Dispatcher.Id}");
                    return await recruitDriverService.RecruitDriverAsync(recruitDriver.Driver,
                        recruitDriver.Dispatcher);
                }

                case ActionType.Delete:
                {
                    logger.LogInformation(
                        $"Fired driver {recruitDriver.Driver.Id} from dispatcher {recruitDriver.Dispatcher.Id}");
                    await recruitDriverService.FireDriverAsync(recruitDriver.Driver, recruitDriver.Dispatcher);
                    break;
                }

                case ActionType.Get:
                {
                    logger.LogInformation(
                        $"Successfully returned drivers for dispatcher [Dispatcher = {recruitDriver.Dispatcher.Id}]");
                    return recruitDriverService.GetDispatcherDriversListAsync(recruitDriver.Dispatcher.Id);
                }

                case ActionType.List:
                {
                    logger.LogInformation($"Successfully returned all drivers without a dispatcher");
                    return recruitDriverService.GetDriverListWoDispatcherAsync();
                }

                default:
                {
                    logger.LogError($"Unknown action {request.Action}");
                    throw new InvalidEnumArgumentException("Unknown action type");
                }
            }

            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            logger.LogInformation(e.Message);
            throw new Exception(e.Message);
        }
    }
}