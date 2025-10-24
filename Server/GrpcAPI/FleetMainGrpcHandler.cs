
using Grpc.Net.Client;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using GrpcAPI.Protos;

namespace GrpcAPI
{
    public class FleetMainGrpcHandler
    {
        private readonly FleetServiceProto.FleetServiceProtoClient _client;
        private const string Host = "http://localhost:6032";
        
        private static readonly Lazy<FleetMainGrpcHandler> _instance =
            new Lazy<FleetMainGrpcHandler>(() =>
            {
                var channel = Grpc.Net.Client.GrpcChannel.ForAddress(Host);
                return new FleetMainGrpcHandler(channel);
            });
        
        private FleetMainGrpcHandler(GrpcChannel channel)
        {
            _client = new FleetServiceProto.FleetServiceProtoClient(channel);
        }

        public static FleetMainGrpcHandler Instance => _instance.Value;
        
        /// <summary>
        /// Sends a request to the gRPC server specifying the handler and action
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The server response</returns>
        public async Task<ResponseProto> SendRequestAsync(RequestProto request)
        {
            try
            {
                return await _client.SendRequestAsync(request);
            }
            catch (Grpc.Core.RpcException ex)
            {
                Console.WriteLine($"RPC failed: {ex.Status}");
                throw;
            }
        }
    }
}