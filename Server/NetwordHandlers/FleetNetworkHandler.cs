namespace NetwordHandlers;
using ApiContracts;
public interface IFleetNetworkHandler
{
    Response HandleAsync(Request request);
}