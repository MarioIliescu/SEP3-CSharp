using ApiContracts;
using ApiContracts.Enums;
using Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PersistanceContracts;
using Repositories;

namespace PersistanceHandlersGrpc.AuthPersistance;

public class AuthHandlerGrpc : IFleetPersistanceHandler
{
    private readonly IAuthRepository _authRepository;
    private readonly ILogger<AuthHandlerGrpc> _logger;
    public AuthHandlerGrpc([FromKeyedServices(HandlerType.Auth)] IAuthRepository authRepository,
        ILogger <AuthHandlerGrpc> logger)
    {
        _logger = logger;
        _authRepository = authRepository;
    }
    public async Task<object> HandleAsync(Request request)
    {
        var user = request.Payload as User ?? throw new ArgumentException(nameof(request.Payload));
        var response = await _authRepository.LoginAsync(user);
        if (response is Dispatcher dispatcher)
        {
            return dispatcher;
        }
        else if (response is Driver driver)
        {
            return driver;
        }
        throw new ArgumentException(nameof(request.Payload));
    }
}