using Entities;
using Google.Protobuf.WellKnownTypes;
using GrpcAPI.GrpcUtils;
using GrpcAPI.Protos;
using Microsoft.Extensions.Logging;
using Repositories;

namespace GrpcAPI.Services;

public class DispatcherServiceProto : IDispatcherRepository
{
    private readonly FleetMainGrpcHandler _handler;
    private readonly ILogger<DispatcherServiceProto> _logger;

    public DispatcherServiceProto(
        FleetMainGrpcHandler fleetMainGrpcHandler,
        ILogger<DispatcherServiceProto> logger)
    {
        _handler = fleetMainGrpcHandler;
        _logger = logger;
    }

    public async Task<Dispatcher> CreateAsync(Dispatcher payload)
    {
        try
        {
            _logger.LogInformation("Creating new user");
            var response = await _handler.SendRequestAsync(
                ProtoUtils.ParseDispatcherRequest(ActionTypeProto.ActionCreate, payload));
            var createdProto = response.Payload.Unpack<DispatcherProto>();
            var created = ProtoUtils.ParseFromProtoToDispatcher(createdProto);
            _logger.LogInformation($"Created new user with Id {created.Id}");
            return await Task.FromResult(created);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating new user");
            throw new Exception(e.Message);
        }
    }

    public async Task UpdateAsync(Dispatcher payload)
    {
        try
        {
            _logger.LogInformation("Updating user");
            await _handler.SendRequestAsync(
                ProtoUtils.ParseDispatcherRequest(ActionTypeProto.ActionUpdate, payload));
            _logger.LogInformation($"Updated new user with Id {payload.Id}");
            await Task.CompletedTask;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw new Exception(e.Message);
        }
    }

    public async Task<Dispatcher> GetSingleAsync(int id)
    {
        // Identify dispatcher by its user id
        DispatcherProto proto = new()
        {
            User = new UserProto
            {
                Id = id
            }
        };

        RequestProto request = new()
        {
            Action  = ActionTypeProto.ActionGet,
            Payload = Any.Pack(proto),
            Handler = HandlerTypeProto.HandlerDispatcher
        };

        _logger.LogInformation("Getting dispatcher for user id {Id}", id);
        var response  = await _handler.SendRequestAsync(request);
        DispatcherProto received = response.Payload.Unpack<DispatcherProto>();
        _logger.LogInformation("Dispatcher received for user id {Id}", received.User?.Id);
        return await Task.FromResult(ProtoUtils.ParseFromProtoToDispatcher(received));
    }

    public async Task DeleteAsync(int id)
    {
        DispatcherProto proto = new()
        {
            User = new UserProto
            {
                Id = id
            }
        };

        RequestProto request = new()
        {
            Action  = ActionTypeProto.ActionDelete,
            Payload = Any.Pack(proto),
            Handler = HandlerTypeProto.HandlerDispatcher
        };

        _logger.LogInformation("Deleting dispatcher for user id {Id}", id);
        await _handler.SendRequestAsync(request);
    }

    public IQueryable<Dispatcher> GetManyAsync()
    {
        _logger.LogInformation("Getting all dispatchers");
        RequestProto request = new RequestProto()
        {
            Handler = HandlerTypeProto.HandlerDispatcher,
            Action = ActionTypeProto.ActionList,
            Payload = Any.Pack(new DispatcherProto()
            {
                User = new UserProto(){Id = 0},
            })
        };
        try
        {
            var response = _handler.SendRequestAsync(request);
            DispatcherListProto received = response.Result.Payload.Unpack<DispatcherListProto>();
            List<Dispatcher> dispatchers = new();
            foreach (DispatcherProto dispatcher in received.Dispatchers)
            {
                dispatchers.Add(ProtoUtils.ParseFromProtoToDispatcher(dispatcher));
            }

            return dispatchers.AsQueryable();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw new Exception(ex.Message);
        }
    }

    
}
