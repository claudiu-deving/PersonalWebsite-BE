using System.Reflection.Metadata;

using ccsflowserver.Data;
using ccsflowserver.Model;

using Microsoft.EntityFrameworkCore;

namespace ccsflowserver.Services;

public class UserService : IModelService<User>
{
    private readonly AppDbContext _dbContext;
    public UserService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public Task<ServiceResponse<User>> Create(User entity)
    {
        throw new Exception("Use the AuthService to create users");
    }

    public async Task<ServiceResponse<bool>> Delete(object id)
    {
        ServiceResponse<bool> serviceResponse = new ServiceResponse<bool>();
        var user = _dbContext.Users.Find(id);
        if (user is null)
        {
            serviceResponse.Success = true;
            serviceResponse.Message = "User deleted!";
            serviceResponse.Data = true;
        }
        else
        {
            _dbContext.Users.Remove(user);
        }
        await _dbContext.SaveChangesAsync();
        return serviceResponse;
    }

    public async Task<ServiceResponse<User>> Get(object id)
    {
        ServiceResponse<User> serviceResponse = new ServiceResponse<User>();
        var user = await _dbContext.Users.FindAsync(id);

        if (user is null)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = "User not found!";
            serviceResponse.Data = null;
        }
        else
        {
            _dbContext.Entry(user).Reference(b => b.Role).Load();
            serviceResponse.Success = true;
            serviceResponse.Message = "User found!";
            serviceResponse.Data = user;
        }
        return serviceResponse;
    }

    public async Task<ServiceResponse<IEnumerable<User>>> Get()
    {
        ServiceResponse<IEnumerable<User>> serviceResponse = new ServiceResponse<IEnumerable<User>>();
        var users = await _dbContext.Users.ToListAsync();
        foreach (var user in users)
        {
            _dbContext.Entry(user).Reference(b => b.Role).Load();
        }
        if (users is null)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = "Users not found!";
            serviceResponse.Data = null;
        }
        else
        {
            serviceResponse.Success = true;
            serviceResponse.Message = "Users found!";
            serviceResponse.Data = users;
        }
        return serviceResponse;
    }

    public async Task<ServiceResponse<User>> Update(User entity)
    {
        ServiceResponse<User> serviceResponse = new();
        var dbUser = await _dbContext.Users.FindAsync(entity);
        if (dbUser is null)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = "User not found!";
            serviceResponse.Data = null;
        }
        else
        {
            dbUser.PassSalt = entity.PassSalt;
            dbUser.PassHash = entity.PassHash;
            dbUser.Username = entity.Username;
            dbUser.Email = entity.Email;
            dbUser.Role = entity.Role;
            _dbContext.Users.Update(dbUser);

            serviceResponse.Success = true;
            serviceResponse.Message = "User updated!";
            serviceResponse.Data = dbUser;
        }
        return serviceResponse;
    }
}
