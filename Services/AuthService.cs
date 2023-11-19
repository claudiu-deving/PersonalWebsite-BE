using ccsflowserver.Data;
using ccsflowserver.Model;

using Microsoft.EntityFrameworkCore;

namespace ccsflowserver.Services;

public class AuthService : IAuthservice
{
    private readonly AppDbContext _appDbContext;
    private readonly IPasswordManager _passwordManager;

    public AuthService(AppDbContext appDbContext, IPasswordManager passwordManager)
    {
        _appDbContext=appDbContext;
        _passwordManager=passwordManager;
    }

    public async Task<bool> UserExists(string username)
    {
        var dbUserName = await _appDbContext.Logins.FirstOrDefaultAsync(u => u.Username==username);
        if(dbUserName is null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    public async Task<bool> Verify(string username, string password)
    {
        var dbUserName = await _appDbContext.Logins.FirstOrDefaultAsync(u => u.Username==username);

        if(dbUserName is null)
        {
            return false;
        }
        var passwordHash = _passwordManager.HashPassword(password, dbUserName.PassSalt);

        var result =Convert.ToBase64String( dbUserName.PassHash).Equals(Convert.ToBase64String(passwordHash));

        return result;
    }

    public async Task<User> RegisterUser(string username, string password)
    {
        var hashedDetails = _passwordManager.HashNewPassword(password);
        User login = new User()
        {
            Username=username,
            PassHash=hashedDetails.hash,
            PassSalt=hashedDetails.salt,
            Id=Guid.NewGuid()
        };
        await _appDbContext.Logins.AddAsync(login);
        await _appDbContext.SaveChangesAsync();
        return login;
    }

}
