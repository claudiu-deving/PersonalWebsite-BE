namespace ccsflowserver.Model;

public sealed class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsAdmin { get; set; } = false;
    public static Role Default => new Role()
    {
        Id=1,
        Name="Guest",
        Description="Basic role",
        IsAdmin=false
    };
}