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
        User user = request.Payload as User ?? throw new ArgumentException(nameof(request.Payload));
        return await _authRepository.LoginAsync(user);
    }
}