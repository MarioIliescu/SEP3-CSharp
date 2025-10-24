namespace PersistanceContracts;
using ApiContracts;
public interface IFleetPersistanceHandler
{
    Task<object> HandleAsync(Request request);
}