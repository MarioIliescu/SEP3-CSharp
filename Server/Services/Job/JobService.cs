namespace Services.Job;

using ApiContracts;
using ApiContracts.Enums;
using PersistanceContracts;
using Entities;
using Microsoft.Extensions.DependencyInjection;

public class JobService : IJobService
{
    private readonly IFleetPersistanceHandler _handler;

    public JobService(
        [FromKeyedServices(HandlerType.Job)] IFleetPersistanceHandler handler)
    {
        _handler = handler;
    }

    public async Task<Job> CreateAsync(Job payload)
    {
        //Logic needed to verify
        Request request = MakeJobRequest(ActionType.Create, payload);
        try
        {
            return (Job)await _handler.HandleAsync(request);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task UpdateAsync(Job payload)
    {
        Request request = MakeJobRequest(ActionType.Update, payload);
        await _handler.HandleAsync(request);
    }

    public async Task<Job> GetSingleAsync(int id)
    {
        Request request = MakeJobRequest(
            ActionType.Get, new Job()
            {
                JobId = id
            }
        );
        return (Job)await _handler.HandleAsync(request);
    }

    public async Task DeleteAsync(int id)
    {
        Request request = MakeJobRequest(
            ActionType.Delete, new Job()
            {
                JobId = id
            }
        );
        await _handler.HandleAsync(request);
    }

    public IQueryable<Job> GetManyAsync()
    {
        Request request = MakeJobRequest(ActionType.List, new Job.Builder()
            .Build());

        var result = _handler.HandleAsync(request).Result as IQueryable<Job>;
        if (result != null)
        {
            return result;
        }

        throw new InvalidOperationException("No jobs found");
    }

    private Request MakeJobRequest(ActionType action, Job job)
    {
        return new Request(action, HandlerType.Job, job);
    }
}