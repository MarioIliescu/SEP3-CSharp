using Entities;
using Google.Protobuf.WellKnownTypes;
using GrpcAPI.GrpcUtils;
using GrpcAPI.Protos;
using Microsoft.Extensions.Logging;
using Repositories;

namespace GrpcAPI.Services;

public class JobServiceProto : IJobRepository
{
    private readonly ILogger<JobServiceProto> _logger;
    private readonly FleetMainGrpcHandler _handler;

    public JobServiceProto(FleetMainGrpcHandler fleetMainGrpcHandler,
        ILogger<JobServiceProto> logger)
    {
        _handler = fleetMainGrpcHandler;
        _logger = logger;
    }

    public async Task<Job> CreateAsync(Job payload)
    {
        _logger.LogInformation("Creating new job");
        try
        {
            var response = await _handler.SendRequestAsync(
                ProtoUtils.ParseJobRequest(ActionTypeProto.ActionCreate,
                    payload));
            JobProto received = response.Payload.Unpack<JobProto>();
            _logger.LogInformation($"Created new job {received.JobId}");
            return await Task.FromResult(
                ProtoUtils.ParseFromProtoToJob(received));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating job");
            throw new Exception(ex.Message);
        }
    }

    public async Task UpdateAsync(Job payload)
    {
        try
        {
            _logger.LogInformation("Updating job");
            await _handler.SendRequestAsync(
                ProtoUtils.ParseJobRequest(ActionTypeProto.ActionUpdate,
                    payload));
            _logger.LogInformation($"Updated new job with Id {payload.JobId}");
            await Task.CompletedTask;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw new Exception(e.Message);
        }
    }

    public async Task<Job> GetSingleAsync(int id)
    {
        _logger.LogInformation("Getting single job");
        try
        {
            var response = await _handler.SendRequestAsync(
                ProtoUtils.ParseJobRequest(ActionTypeProto.ActionGet,
                    new Job
                            .Builder()
                        .SetId(id)
                        .Build()));
            var jobProto = response.Payload.Unpack<JobProto>();
            var job = ProtoUtils.ParseFromProtoToJob(jobProto);
            _logger.LogInformation($"Getting single job with Id {id}");
            return await Task.FromResult(job);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw new Exception(ex.Message);
        }
    }

    public async Task DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting job");
        try
        {
            await _handler.SendRequestAsync(ProtoUtils.ParseJobRequest(
                ActionTypeProto.ActionDelete,
                new Job
                        .Builder()
                    .SetId(id)
                    .Build()));
            _logger.LogInformation($"Deleted job with Id {id}");
            await Task.CompletedTask;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw new Exception(e.Message);
        }
    }

    public IQueryable<Job> GetManyAsync()
    {
        _logger.LogInformation("Getting all jobs...");


        RequestProto request = new RequestProto()
        {
            Handler = HandlerTypeProto.HandlerJob,
            Action = ActionTypeProto.ActionList,
            Payload = Any.Pack(new JobProto()
            {
                JobId = 0,
            })
        };

        try
        {
            var response = _handler.SendRequestAsync(request);
            JobListProto received =
                response.Result.Payload.Unpack<JobListProto>();

            List<Job> jobs = new();

            foreach (JobProto job in received.Jobs)
            {
                jobs.Add(ProtoUtils.ParseFromProtoToJob(job));
            }


            _logger.LogInformation($"Jobs returned {received.Jobs.Count}");
            return jobs.AsQueryable();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting all jobs");
            throw new Exception(e.Message);
        }
    }
}