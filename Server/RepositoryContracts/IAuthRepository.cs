using Entities;

namespace Repositories;

public interface IAuthRepository 
{
    Task<object> LoginAsync(User user);
}