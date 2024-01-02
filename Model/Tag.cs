using ccsflowserver.Services;

using System.ComponentModel.DataAnnotations.Schema;

namespace ccsflowserver.Model;

[Table("tags")]
public class Tag : IEntity
{
    public required int Id { get; set; }

    public required string Name { get; set; }

    public string Color { get; set; } = "#FFFFFF";

    public string Description { get; set; } = string.Empty;

    public virtual ICollection<TagBlogpostMapping>? Blogs { get; set; }

    object IEntity.Id => Id;
}
