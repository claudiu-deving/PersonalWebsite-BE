namespace ccsflowserver.Model;

public class TagDTO
{
    public TagDTO(int id, string name, string color, string description, int blogCount)
    {
        Id = id;
        Name = name;
        Color = color;
        Description = description;
        BlogCount = blogCount;
    }

    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Color { get; set; } = "#FFFFFF";

    public string Description { get; set; } = string.Empty;

    public int BlogCount { get; set; }
}