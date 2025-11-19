using Entities;
using Google.Protobuf.WellKnownTypes;
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
        DispatcherProto proto = MapDispatcherToProto(payload);

        RequestProto request = new()
        {
            Action  = ActionTypeProto.ActionCreate,
            Payload = Any.Pack(proto),
            Handler = HandlerTypeProto.HandlerDispatcher
        };

        _logger.LogInformation("Creating new dispatcher");
        var response  = await _handler.SendRequestAsync(request);
        DispatcherProto received = response.Payload.Unpack<DispatcherProto>();
        _logger.LogInformation("Created dispatcher for user id {Id}", received.User?.Id);

        return MapProtoToDispatcher(received);
    }

    public async Task UpdateAsync(Dispatcher payload)
    {
        DispatcherProto proto = MapDispatcherToProto(payload);

        RequestProto request = new()
        {
            Action  = ActionTypeProto.ActionUpdate,
            Payload = Any.Pack(proto),
            Handler = HandlerTypeProto.HandlerDispatcher
        };

        _logger.LogInformation("Updating dispatcher for user id {Id}", proto.User?.Id);
        await _handler.SendRequestAsync(request);
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

        return MapProtoToDispatcher(received);
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
        DispatcherProto proto = new()
        {
            CurrentRate = 0,
            User = new UserProto
            {
                Id          = 0,
                FirstName   = "default",
                LastName    = "default",
                Email       = "default@example.com",
                PhoneNumber = "+0",
                Password    = "default"
            }
        };

        RequestProto request = new()
        {
            Action  = ActionTypeProto.ActionList,
            Handler = HandlerTypeProto.HandlerDispatcher,
            Payload = Any.Pack(proto)
        };

        _logger.LogInformation("Getting all dispatchers");

        var response = _handler.SendRequestAsync(request); // sync wait like CompanyServiceProto
        DispatcherListProto received = response.Result.Payload.Unpack<DispatcherListProto>();

        List<Dispatcher> dispatchers = new();

        foreach (DispatcherProto d in received.Dispatchers)
        {
            dispatchers.Add(MapProtoToDispatcher(d));
        }

        _logger.LogInformation("Dispatchers returned {Count}", dispatchers.Count);
        return dispatchers.AsQueryable();
    }

    // ----------------- mapping helpers -----------------

    private static DispatcherProto MapDispatcherToProto(Dispatcher dispatcher)
    {
        // Commission_Rate is int; CurrentRate in proto is double
        var userProto = new UserProto
        {
            Id          = dispatcher.Id,
            FirstName   = dispatcher.FirstName,
            LastName    = dispatcher.LastName,
            Email       = dispatcher.Email,
            PhoneNumber = dispatcher.PhoneNumber,
            Password    = dispatcher.Password // decide if you want to send this or not
        };

        return new DispatcherProto
        {
            CurrentRate = dispatcher.Current_Rate,
            User        = userProto
            // DriversAssigned left empty for now – you can add that later if needed
        };
    }

    private static Dispatcher MapProtoToDispatcher(DispatcherProto proto)
    {
        var user = proto.User;

        var builder = new Dispatcher.Builder();

        if (user != null)
        {
            builder.SetId(user.Id);
            builder.SetFirstName(user.FirstName);
            builder.SetLastName(user.LastName);
            builder.setEmail(user.Email);
            builder.SetPhoneNumber(user.PhoneNumber);
        }

        builder.SetCurrentRate((int)proto.CurrentRate);

        return builder.Build();
    }
}
