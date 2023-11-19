namespace ccsflowserver.Model;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public required byte[] PassHash { get; set; }
    public required byte[] PassSalt { get; set; }
}
