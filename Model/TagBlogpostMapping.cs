using ccsflowserver.Services;

using System.ComponentModel.DataAnnotations.Schema;

namespace ccsflowserver.Model;

[Table("tagBlogpostMappings")]
public class TagBlogpostMapping : IEntity
{
    public int Id { get; set; }

    public required int TagId { get; set; }

    public required int BlogpostId { get; set; }

    public virtual BlogPost? VirtualBlogposts { get; set; }

    public virtual Tag? Tag { get; set; }

    object IEntity.Id => Id;

}
