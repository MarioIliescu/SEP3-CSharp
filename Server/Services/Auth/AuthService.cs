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
    public async Task<object> LoginAsync(User user)
    {
        _logger.LogDebug("Attempting login");
        var request = new Request(ActionType.Get, HandlerType.Auth, user);
        var fetchedUser = await _authHandler.HandleAsync(request) ?? 
            throw new Exception("User not found, please register");
        _logger.LogDebug("Found user");
        var returnedUser = fetchedUser ?? throw new Exception("User not found, please register");
        if (returnedUser is Entities.Dispatcher dispatcher)
        {
            if (PasswordHasher.Verify(user.Password, dispatcher.Password))
            {
                return dispatcher;
            }
            _logger.LogDebug("Wrong password");
            throw new Exception("Incorrect password, please try again");
        }
        else if (returnedUser is Entities.Driver driver)
        {
            if (PasswordHasher.Verify(user.Password, driver.Password))
            {
                return driver;
            }
            _logger.LogDebug("Wrong password");
            throw new Exception("Incorrect password, please try again");
        }
        _logger.LogDebug("Something when wrong. Try again");
        throw new Exception("Something when wrong. Try again");
    }
}