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
    public async Task<User> LoginAsync(User user)
    {
        _logger.LogInformation("Attempting log in");
        var response = await _handler.SendRequestAsync(
            new RequestProto()
            {
                Handler = HandlerTypeProto.HandlerAuth,
                Action = ActionTypeProto.ActionGet,
                Payload = Any.Pack(ProtoUtils.ParseUserToProto(user)),
            });
        UserProto userProto = response.Payload.Unpack<UserProto>();
        return ProtoUtils.ParseFromProtoToUser(userProto); ;
    }
}