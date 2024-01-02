using ccsflowserver.Services;

namespace ccsflowserver.Model;

public class Category : IEntity
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public virtual ICollection<BlogPost>? BlogPosts { get; set; }
    object IEntity.Id => Id;

}
