using Entities;

namespace Repositories;

public interface IAuthRepository 
{
    Task<User> LoginAsync(User user);
}