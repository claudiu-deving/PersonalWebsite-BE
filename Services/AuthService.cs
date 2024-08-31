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
		_appDbContext = appDbContext;
		_passwordManager = passwordManager;
	}

	public async Task<bool> UserExists(Guid id)
	{
		var dbUserName = await _appDbContext.Users.FindAsync(id);
		if (dbUserName is null)
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
		if (dbUserName is null)
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
		var dbUser = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Username == username);

		if (dbUser is null)
		{
			return null;
		}
		_appDbContext.Entry(dbUser).Reference(u => u.Role).Load();
		var passwordHash = _passwordManager.HashPassword(password, dbUser.PassSalt);

		var result = Convert.ToBase64String(dbUser.PassHash).Equals(Convert.ToBase64String(passwordHash));

		return dbUser;
	}

	public async Task<User> RegisterUser(UserPayloadRegistration user)
	{
		var (hash, salt) = _passwordManager.HashNewPassword(user.Password);
		var authorRole = _appDbContext.Roles.FirstOrDefault(x => x.Name == "Author");
		authorRole ??= new Role()
		{
			Id = 0,
			Name = "Author",
			IsAdmin = false
		};
		User login = new()
		{
			Username = user.Username,
			PassHash = hash,
			PassSalt = salt,
			Email = user.Email,
			Id = Guid.NewGuid(),
			Role = authorRole,
			RoleId = authorRole.Id
		};
		await _appDbContext.Users.AddAsync(login);
		await _appDbContext.SaveChangesAsync();
		return login;
	}

	public Task<bool> EmailExists(string email)
	{
		if (_appDbContext.Users.Any(u => u.Email == email))
		{
			return Task.FromResult(true);
		}
		else
		{
			return Task.FromResult(false);
		}
	}
}
