using System.ComponentModel;
using ApiContracts.Enums;
using Entities;
using Google.Protobuf.WellKnownTypes;
using GrpcAPI.GrpcUtils;
using GrpcAPI.Protos;
using Microsoft.Extensions.Logging;
using Repositories;
namespace GrpcAPI.Services;

public class AuthServiceProto: IAuthRepository
{
    private readonly FleetMainGrpcHandler _handler;
    private readonly ILogger<AuthServiceProto> _logger;
    public AuthServiceProto(FleetMainGrpcHandler handler, ILogger<AuthServiceProto> logger)
    {
        _handler = handler;
        _logger = logger;
    }
    public async Task<object> LoginAsync(User user)
    {
        _logger.LogInformation("Attempting log in");
        var response = await _handler.SendRequestAsync(
            new RequestProto()
            {
                Handler = HandlerTypeProto.HandlerAuth,
                Action = ActionTypeProto.ActionGet,
                Payload = Any.Pack(ProtoUtils.ParseUserToProto(user)),
            });
        var any = response.Payload;
        string type = any.TypeUrl;

        _logger.LogDebug("Payload TypeUrl returned: {TypeUrl}", type);
        if (type.EndsWith("DriverProto"))
        {
            var driverProto = any.Unpack<DriverProto>();
            _logger.LogInformation("Driver authenticated");
            return ProtoUtils.ParseFromProtoToDriver(driverProto);
        }
        else if (type.EndsWith("DispatcherProto"))
        {
           var dispatcherProto = any.Unpack<DispatcherProto>();
           _logger.LogInformation("Dispatcher authenticated");
            return ProtoUtils.ParseFromProtoToDispatcher(dispatcherProto);
        }

        throw new Exception($"Unsupported user type returned: {type}");
    }
}