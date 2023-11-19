using System.Text.Json.Serialization;

using ccsflowserver.Services;

namespace ccsflowserver.Model;

public class BlogPost : IEntity
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    object IEntity.Id { get => Id; set => Id=(int)value; }
    public DateTime Modified { get; set; }
    public BlogPost(int id, string title, string content, DateTime created, DateTime modified)
    {
        Id=id;
        Title=title;
        Content=content;
        Created=created;
        Modified=modified;
    }

}
