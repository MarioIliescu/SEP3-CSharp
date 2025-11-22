using ApiContracts;
using ApiContracts.Enums;
using Entities;
using FleetWebApi.SecurityUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PersistanceContracts;
namespace Services.Auth;

public class AuthService : IAuthService
{
    private readonly IFleetPersistanceHandler _authHandler;
    private ILogger<AuthService> _logger;

    public AuthService([FromKeyedServices(HandlerType.Auth)]IFleetPersistanceHandler authHandler, ILogger<AuthService> logger)
    {
        _authHandler = authHandler;
        _logger = logger;
    }
    public async Task<User> LoginAsync(User user)
    {
        _logger.LogDebug("Attempting login");
        var request = new Request(ActionType.Get, HandlerType.Auth, user);
        var fetchedUser = await _authHandler.HandleAsync(request) ?? 
            throw new Exception("User not found, please register");
        _logger.LogDebug("Found user");
        User returnedUser = fetchedUser as User ?? throw new Exception("User not found, please register");
        if (PasswordHasher.Verify(user.Password, returnedUser.Password))
        {
            return returnedUser;
        }
        _logger.LogDebug("Wrong password");
        throw new Exception("Incorrect password, please try again");
    }
}