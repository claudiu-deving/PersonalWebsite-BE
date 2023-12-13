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

    public async Task<bool> UserExists(Guid id)
    {
        var dbUserName = await _appDbContext.Users.FindAsync(id);
        if(dbUserName is null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public async Task<bool> UserExists(string username)
    {
        var dbUserName = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Username.Equals(username));
        if(dbUserName is null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    public async Task<User?> Verify(string username, string password)
    {
        var dbUserName = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Username==username);

        if(dbUserName is null)
        {
            return null;
        }
        var passwordHash = _passwordManager.HashPassword(password, dbUserName.PassSalt);

        var result = Convert.ToBase64String(dbUserName.PassHash).Equals(Convert.ToBase64String(passwordHash));

        return dbUserName;
    }

    public async Task<User> RegisterUser(UserPayloadRegistration user)
    {
        var hashedDetails = _passwordManager.HashNewPassword(user.Password);
        var authorRole = _appDbContext.Roles.FirstOrDefault(x => x.Name=="Author");
        if(authorRole is null)
        {
            authorRole=new Role()
            {
                Id=0,
                Name="Author",
                IsAdmin=false
            };
        }
        User login = new User()
        {
            Username=user.Username,
            PassHash=hashedDetails.hash,
            PassSalt=hashedDetails.salt,
            Email=user.Email,
            Id=Guid.NewGuid(),
            Role=authorRole,
            RoleId=authorRole.Id
        };
        await _appDbContext.Users.AddAsync(login);
        await _appDbContext.SaveChangesAsync();
        return login;
    }

    public Task<bool> EmailExists(string email)
    {
        if(_appDbContext.Users.Any(u => u.Email==email))
        {
            return Task.FromResult(true);
        }
        else
        {
            return Task.FromResult(false);
        }
    }
}
