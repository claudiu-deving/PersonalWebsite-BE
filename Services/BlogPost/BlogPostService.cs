using System.Reflection.Metadata;
using System.Text.RegularExpressions;

using ccsflowserver.Data;
using ccsflowserver.Model;


using Microsoft.EntityFrameworkCore;

namespace ccsflowserver.Services;

public class BlogPostService : IModelService<BlogPost>
{
	private readonly AppDbContext _appDbContext;

	public BlogPostService(AppDbContext appDbContext)
	{
		_appDbContext = appDbContext;
	}


	public async Task<ServiceResponse<BlogPost>> Create(BlogPost entity)
	{
		ServiceResponse<BlogPost> response = new();
		entity.Created = DateTime.Now.ToUniversalTime();
		entity.Modified = DateTime.Now.ToUniversalTime();
		var existingCategory = _appDbContext.Categories.Find(entity.CategoryId);

		if (entity.Category is null )
		{
			var defaultCategory = _appDbContext.Categories.FirstOrDefault() ??
				new Category() { Name = "Default", Description = "The default category" };
			entity.Category = defaultCategory;
		}

		await _appDbContext.AddAsync(entity);
		var rowsAffected = await _appDbContext.SaveChangesAsync();
		if (rowsAffected == 1)
		{
			response.Success = true;
			response.Message = $"Blog post  {entity.Title} with Id {entity.Id} created succesfully";
			response.Data = entity;
		}
		else
		{
			response.Success = false;
			response.Message = $"Unable to create blog post  {entity.Title}";
			response.Data = entity;
		}
		return response;
	}

	public async Task<ServiceResponse<bool>> Delete(object id)
	{
		var response = new ServiceResponse<bool>();

		// Validate input id
		if (id == null || !(id is int) || (int)id < 0)
		{
			response.Success = false;
			response.Message = "Invalid ID provided.";
			response.Data = false;
			return response;
		}

		try
		{
			var blogPost = await _appDbContext.BlogPosts.FirstOrDefaultAsync(b => b.Id == (int)id);
			if (blogPost is null)
			{
				response.Success = true;
				response.Message = $"No blog post found with id: {id}, no action taken.";
				response.Data = true;
				return response;
			}

			_appDbContext.BlogPosts.Remove(blogPost);
			var rowsAffected = await _appDbContext.SaveChangesAsync();

			if (rowsAffected > 1)
			{
				throw new InvalidOperationException($"Multiple blog posts deleted with id: {id}");
			}

			response.Success = true;
			response.Message = $"Blog post with Id {id} successfully deleted.";
			response.Data = true;
		}
		catch (DbUpdateConcurrencyException ex)
		{
			response.Success = false;
			response.Message = "The blog post could not be deleted due to a concurrency issue.";
			response.Data = false;
		}
		catch (Exception ex)
		{
			response.Success = false;
			response.Message = "An error occurred while deleting the blog post.";
			response.Data = false;
		}

		return response;
	}



	public async Task<ServiceResponse<BlogPost>> Get(object id)
	{
		var response = new ServiceResponse<BlogPost>();
		var blog = await _appDbContext.BlogPosts.FindAsync(id);
		var users = await _appDbContext.Users.ToListAsync();


		if (blog is null)
		{
			response.Success = false;
			response.Message = $"Unable to find the blog post with Id: {id}";
			response.Data = blog;
		}
		else
		{
			_appDbContext.Entry(blog).Reference(b => b.Author).Load();
			var author = users.FirstOrDefault(u => u.Id == blog.AuthorId);
			if (author != null)
			{
				blog.Author = author;
				_appDbContext.Entry(author).Reference(b => b.Role).Load();
			}

			blog.Content = string.Join(Environment.NewLine, blog.Content);
			response.Success = true;
			response.Message = $"Blog post with Id: {id} found";
			response.Data = blog;
		}
		return response;
	}

	public async Task<ServiceResponse<IEnumerable<BlogPost>>> Get()
	{
		var response = new ServiceResponse<IEnumerable<BlogPost>>();
		var data = await _appDbContext.BlogPosts.ToListAsync();
		var users = await _appDbContext.Users.ToListAsync();

		data.ForEach(blog =>
		{
			_appDbContext.Entry(blog).Reference(b => b.Author).Load();
			_appDbContext.Entry(blog).Reference(b => b.Category).Load();

			_appDbContext.Entry(blog).Collection(b => b.Tags).Load();
			foreach (var tag in blog.Tags!)
			{
				_appDbContext.Entry(tag).Reference(b => b.Tag).Load();
			}
			var author = users.FirstOrDefault(u => u.Id == blog.AuthorId);
			if (author != null)
			{
				blog.Author = author;
				_appDbContext.Entry(author).Reference(b => b.Role).Load();
			}
		});
		if (data is null)
		{
			response.Success = false;
			response.Message = $"Unable to retrieve the blog posts";
			response.Data = data;
		}
		else
		{
			data.ForEach(blog =>
			{
				blog.Content = string.Join(Environment.NewLine, blog.Content);
			});
			response.Success = true;
			response.Message = $"Blog posts retrieved";
			response.Data = data;
		}
		return response;
	}

	public async Task<ServiceResponse<BlogPost>> Update(BlogPost entity)
	{
		var response = new ServiceResponse<BlogPost>();
		var data = await _appDbContext.BlogPosts.FindAsync(entity.Id);
		if (data is null)
		{
			response.Success = false;
			response.Message = $"Unable to find the blog post with Title: {entity.Title}";
			response.Data = data;
		}
		else
		{
			data.Title = entity.Title;
			data.Content = entity.Content;
			data.Modified = DateTime.Now.ToUniversalTime();

			var rowsAffected = await _appDbContext.SaveChangesAsync();
			if (rowsAffected == 1)
			{
				response.Success = true;
				response.Message = $"Blog post with Title: {entity.Title} updated succesfully";
				response.Data = data;
			}
			else
			{
				response.Success = false;
				response.Message = $"Unable to update blog post with Title: {entity.Title}";
				response.Data = data;
			}
		}
		return response;
	}
}
