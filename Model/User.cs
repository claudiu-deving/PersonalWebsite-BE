using ccsflowserver.Services;

namespace ccsflowserver.Model;

public class User : IEntity
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public required byte[] PassHash { get; set; }
    public required byte[] PassSalt { get; set; }
    public string Email { get; set; } = string.Empty;
    public Role? Role { get; set; }
    object IEntity.Id { get => Id; }
}

