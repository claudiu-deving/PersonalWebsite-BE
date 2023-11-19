using ccsflowserver.Model;

namespace ccsflowserver.Services;

public interface IAuthservice
{
    Task<bool> Verify(string username, string password);
    Task<User> RegisterUser(string username, string password);
    Task<bool> UserExists(string username);
}
