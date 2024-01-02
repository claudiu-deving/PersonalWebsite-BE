namespace ccsflowserver.Model;

public class BlogPostCreate
{
    /// <summary>
    /// Title of the blog
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Specially formatted content
    /// </summary>
    public required string Content { get; set; }

    public string? Category { get; set; }

    public IEnumerable<string>? Tags { get; set; }
}



