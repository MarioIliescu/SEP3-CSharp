
using Grpc.Net.Client;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using GrpcAPI.Protos;

namespace GrpcAPI
{
    public class FleetMainGrpcHandler
    {
        private readonly FleetServiceProto.FleetServiceProtoClient _client;

        public FleetMainGrpcHandler(GrpcChannel channel)
        {
            _client = new FleetServiceProto.FleetServiceProtoClient(channel);
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
                ResponseProto  response= await _client.SendRequestAsync(request);
                switch (response.Status)
                {
                    case StatusTypeProto.StatusOk:
                        return  response;
                    case StatusTypeProto.StatusError:
                        var errorString = response.Payload.Unpack<StringValue>().Value;
                        throw new Exception($"{response.Status}: {errorString}");
                    case StatusTypeProto.StatusInvalidPayload:
                        var errorString2 = response.Payload.Unpack<StringValue>().Value;
                        throw new InvalidDataException($"{response.Status}: {errorString2}");
                }
            }
            catch (Grpc.Core.RpcException ex)
            {
                Console.WriteLine($"RPC failed: {ex.Status}");
                throw;
            }
            throw new Exception("Grpc call failed");
        }
    }
}
