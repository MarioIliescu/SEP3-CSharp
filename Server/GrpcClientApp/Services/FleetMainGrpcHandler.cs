
using Grpc.Net.Client;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using GrpcClientApp.Protos;

namespace GrpcClientApp.Services
{
    public class FleetMainGrpcHandler
    {
        private readonly FleetService.FleetServiceClient _client;

        public FleetMainGrpcHandler(GrpcChannel channel)
        {
            _client = new FleetService.FleetServiceClient(channel);
        }

        /// <summary>
        /// Sends a request to the gRPC server specifying the handler and action
        /// </summary>
        /// <param name="handler">Which handler the server should use</param>
        /// <param name="action">Action type</param>
        /// <param name="payload">Any protobuf message to send</param>
        /// <returns>The server response</returns>
        public async Task<Response> SendRequestAsync(HandlerType handler, ActionType action, IMessage payload)
        {
            var request = new Request
            {
                Handler = handler,
                Action = action,
                Payload = Any.Pack(payload)
            };

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