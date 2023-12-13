namespace ccsflowserver.Model;

public class UserPayloadVerification
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UserPayloadRegistration
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class UserPayloadName
{
    public string Username { get; set; } = string.Empty;
}

public class UserPayload
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;

}
