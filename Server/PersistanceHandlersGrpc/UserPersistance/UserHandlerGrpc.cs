using ApiContracts;
using PersistanceContracts;

namespace PersistanceHandlersGrpc.UserPersistance;

public class UserHandlerGrpc : IFleetPersistanceHandler
{
    public Task<object> HandleAsync(Request request)
    {
        throw new NotImplementedException();
    }
}