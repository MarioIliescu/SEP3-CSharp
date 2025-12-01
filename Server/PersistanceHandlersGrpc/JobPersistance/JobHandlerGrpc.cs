using System.ComponentModel;
using ApiContracts;
using ApiContracts.Enums;
using Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PersistanceContracts;
using PersistanceHandlersGrpc.CompanyPersistance;
using Repositories;

namespace PersistanceHandlersGrpc.JobPersistance;

public class JobHandlerGrpc : IFleetPersistanceHandler
{
    private readonly IJobRepository _jobService;
    private readonly ILogger<JobHandlerGrpc> _logger;

    public JobHandlerGrpc(
        [FromKeyedServices(HandlerType.Job)] IJobRepository jobService,
        ILogger<JobHandlerGrpc> logger)
    {
        this._jobService = jobService;
        this._logger = logger;
    }

    public async Task<object> HandleAsync(Request request)
    {
        try
        {
            var job = (Job)request.Payload ??
                      throw new ArgumentNullException(nameof(request.Payload));
            switch (request.Action)
            {
                case ActionType.Create:
                {
                    _logger.LogInformation($"Created job {job}");
                    return await _jobService.CreateAsync(job);
                }
                case ActionType.Update:
                {
                    await _jobService.UpdateAsync(job);
                    _logger.LogInformation($"Updated job {job}");
                    break;
                }
                case ActionType.Delete:
                {
                    await _jobService.DeleteAsync(job.jobId);
                    _logger.LogInformation($"Deleted job {job.jobId}");
                    break;
                }
                case ActionType.Get:
                {
                    _logger.LogInformation($"Get job {job.jobId}");
                    return await _jobService.GetSingleAsync(job.jobId);
                }
                case ActionType.List:
                {
                    _logger.LogInformation($"List job {job.jobId}");
                    return _jobService.GetManyAsync();
                }
                default:
                    _logger.LogError($"Unknown action {request.Action}");
                    throw new InvalidEnumArgumentException(
                        "Unknown action type");
            }

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw new Exception(ex.Message);
        }
    }
}