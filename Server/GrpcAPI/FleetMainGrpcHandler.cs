
using Grpc.Net.Client;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using GrpcAPI.Protos;

namespace GrpcAPI
{
    public class FleetMainGrpcHandler
    {
        private readonly FleetService.FleetServiceClient _client;
        private const string Host = "http://localhost:6032";
        GrpcChannel channel = GrpcChannel.ForAddress(Host);
        public FleetMainGrpcHandler()
        {
            _client = new FleetService.FleetServiceClient(channel);
        }

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