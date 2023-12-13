using ccsflowserver.Model;

namespace ccsflowserver.Services;

public interface IAuthservice
{
    Task<User?> Verify(string username, string password);
    Task<User> RegisterUser(UserPayloadRegistration user);
    Task<bool> UserExists(Guid id);
    Task<bool> UserExists(string username);
    Task<bool> EmailExists(string email);
}
