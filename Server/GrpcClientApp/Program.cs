using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using GrpcClientApp.Protos;
using var channel = GrpcChannel.ForAddress("http://localhost:6032");
var handler = new FleetService.FleetServiceClient(channel);

var request = new Request
{
    Handler = HandlerType.HandlerCompany,
    Action = ActionType.ActionCreate,
    Payload = Any.Pack(new CompanyProto()
    {
        McNumber = "0123456789",
        CompanyName = "Carolina SRL"
    })
};

        var response = await handler.SendRequestAsync(request);

        Console.WriteLine($"Response received! {response}");