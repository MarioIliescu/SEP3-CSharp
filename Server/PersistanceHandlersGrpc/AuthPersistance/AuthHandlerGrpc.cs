using ApiContracts;
using PersistanceContracts;

namespace PersistanceHandlersGrpc.AuthPersistance;

public class AuthHandlerGrpc : IFleetPersistanceHandler
{
    public Task<object> HandleAsync(Request request)
    {
        throw new NotImplementedException();
    }
}