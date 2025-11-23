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
    //private readonly DispatcherServiceProto _dispatcherService;
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
        switch (user.Role)
        {
            case UserRole.DRIVER:
            {
                DriverProto proto = response.Payload.Unpack<DriverProto>();
                _logger.LogDebug("Found driver");
                return ProtoUtils.ParseFromProtoToDriver(proto);
            }
            case UserRole.DISPATCHER:
            {
                throw new NotImplementedException();
            }
            default:
                throw new InvalidEnumArgumentException("Invalid role");
        }
    }
}