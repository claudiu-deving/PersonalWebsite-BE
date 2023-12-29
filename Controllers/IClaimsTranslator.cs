using System.Security.Claims;

namespace ccsflowserver.Controllers;

public interface IClaimsTranslator
{
    Guid GetUserId(ClaimsPrincipal user);
    string? GetUserName(ClaimsPrincipal principal);
    bool IsAdmin(ClaimsPrincipal user);
}

public class ClaimsTranslator : IClaimsTranslator
{
    public Guid GetUserId(ClaimsPrincipal user)
    {
        var id = user.FindFirst("id");
        if(id is null)
        {
            return Guid.Empty;
        }
        return Guid.Parse(id.Value);
    }

    public string? GetUserName(ClaimsPrincipal principal)
    {
        var name = principal.FindFirst(ClaimTypes.Name);
        if(name is null)
        {
            return null;
        }
        return name.Value;
    }

    public bool IsAdmin(ClaimsPrincipal principal)
    {
        var role = principal.FindFirst(ClaimTypes.Role)?.Value;
        if(role is null)
        {
            return false;
        }
        if(role=="Admin")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}