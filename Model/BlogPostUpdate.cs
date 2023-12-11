using ccsflowserver.Services;

namespace ccsflowserver.Model;

/// <summary>
/// DTO used for updating the BlogPosts without the need to update all the fields and exposing critical data
/// </summary>
public class BlogPostUpdate
{
    /// <summary>
    /// Title of the blog
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Specially formatted content
    /// </summary>
    public string? Content { get; set; }

    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public UserPayloadName? Author { get; set; }
}
