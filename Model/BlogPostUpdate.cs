using ccsflowserver.Services;

namespace ccsflowserver.Model;

/// <summary>
/// DTO used for updating the BlogPosts without the need to update all the fields and exposing critical data
/// </summary>
public class BlogPostUpdate
{
    public int Id { get; set; }
    /// <summary>
    /// Title of the blog
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Specially formatted content
    /// </summary>
    public string? Content { get; set; }

    public DateTime? Created { get; set; }
    public DateTime? Modified { get; set; }
    public UserPayload? Author { get; set; }
    public bool IsApproved { get; set; }
}

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
}



