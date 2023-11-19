using System.IdentityModel.Tokens.Jwt;
using System.Text;

using ccsflowserver.Model;
using ccsflowserver.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ccsflowserver.AA;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IAuthservice _authService;

    public AuthController(IConfiguration configuration, IAuthservice authService)
    {
        _configuration=configuration;
        _authService=authService;
    }

    [HttpPost("token")]
    public async Task<IActionResult> GenerateToken(UserPayload user)
    {
        // Validate the user credentials
        // For demo purposes, we are assuming the user is authenticated.
        // In a real scenario, you should verify the username and password against a database or another service.

        if(!await _authService.Verify(user.Username, user.Password))
        {
            return Unauthorized();
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])),
            signingCredentials: credentials
        );

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration = token.ValidTo
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(UserPayload user)
    {
        if(string.IsNullOrEmpty(user.Username)||string.IsNullOrEmpty(user.Password))
            return BadRequest("Username or password is missing");

        if(await _authService.UserExists(user.Username))
        {
            return BadRequest("Username already exists");
        }

        var userResponse = await _authService.RegisterUser(user.Username, user.Password);

        return Ok(userResponse);
    }
}
