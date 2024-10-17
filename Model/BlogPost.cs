using System.Text.Json.Serialization;

using ccsflowserver.Services;

namespace ccsflowserver.Model;

public class BlogPost : IEntity
{
	public int Id { get; set; }
	public string? Title { get; set; }
	public string Content { get; set; } = string.Empty;
	public DateTime Created { get; set; }
	object IEntity.Id { get => Id; }
	public DateTime Modified { get; set; }
	public Guid AuthorId { get; set; }
	public virtual User Author { get; set; }
	public string HeroImagePath { get; set; } = string.Empty;

	public string Slug { get; set; } = string.Empty;
	public bool IsApproved { get; set; }

	public int CategoryId { get; set; }
	public virtual Category? Category { get; set; }

	public ICollection<TagBlogpostMapping>? Tags { get; set; }

	public BlogPost()
	{

	}
	public BlogPost(string title, string content, User author)
	{
		Title = title;
		Content = content;
		Created = DateTime.Now.ToUniversalTime();
		Modified = DateTime.Now.ToUniversalTime();
		AuthorId = author.Id;
	}

}
