using System.Text.RegularExpressions;

using ccsflowserver.Data;
using ccsflowserver.Model;

using MdToHtml;

using Microsoft.EntityFrameworkCore;

namespace ccsflowserver.Services;

public class BlogPostService : IModelService<BlogPost>
{
    private readonly AppDbContext _appDbContext;

    public BlogPostService(AppDbContext appDbContext)
    {
        _appDbContext=appDbContext;
    }


    public async Task<ServiceResponse<BlogPost>> Create(BlogPost entity)
    {
        ServiceResponse<BlogPost> response = new ServiceResponse<BlogPost>();
        entity.Created=DateTime.Now.ToUniversalTime();
        entity.Modified=DateTime.Now.ToUniversalTime();

        await _appDbContext.AddAsync(entity);
        var rowsAffected = await _appDbContext.SaveChangesAsync();
        if(rowsAffected==1)
        {
            response.Success=true;
            response.Message=$"Blog post  {entity.Title} with Id {entity.Id} created succesfully";
            response.Data=entity;
        }
        else
        {
            response.Success=false;
            response.Message=$"Unable to create blog post  {entity.Title}";
            response.Data=entity;
        }
        return response;
    }

    public async Task<ServiceResponse<bool>> Delete(object id)
    {
        var response = new ServiceResponse<bool>();

        // Validate input id
        if(id==null||!(id is int)||(int)id<0)
        {
            response.Success=false;
            response.Message="Invalid ID provided.";
            response.Data=false;
            return response;
        }

        try
        {
            var blogPost = await _appDbContext.BlogPosts.FirstOrDefaultAsync(b => b.Id==(int)id);
            if(blogPost is null)
            {
                response.Success=true; // Idempotency: the resource already doesn't exist
                response.Message=$"No blog post found with id: {id}, no action taken.";
                response.Data=true; // Indicates that the end state is as desired: no such blog post
                return response;
            }

            _appDbContext.BlogPosts.Remove(blogPost);
            var rowsAffected = await _appDbContext.SaveChangesAsync();

            if(rowsAffected>1)
            {
                // Handle the case where more than one row is affected
                throw new InvalidOperationException($"Multiple blog posts deleted with id: {id}");
            }

            response.Success=true;
            response.Message=$"Blog post with Id {id} successfully deleted.";
            response.Data=true;
        }
        catch(DbUpdateConcurrencyException ex)
        {
            response.Success=false;
            response.Message="The blog post could not be deleted due to a concurrency issue.";
            response.Data=false;
            // Log the exception details for debugging purposes
        }
        catch(Exception ex)
        {
            response.Success=false;
            response.Message="An error occurred while deleting the blog post.";
            response.Data=false;
            // Log the exception details for debugging purposes
        }

        return response;
    }



    public async Task<ServiceResponse<BlogPost>> Get(object id, bool parse = true)
    {
        var response = new ServiceResponse<BlogPost>();
        var data = await _appDbContext.BlogPosts.FindAsync(id);

        if(data is null)
        {
            response.Success=false;
            response.Message=$"Unable to find the blog post with Id: {id}";
            response.Data=data;
        }
        else
        {
            data.Content=string.Join(Environment.NewLine, data.Content);
            response.Success=true;
            response.Message=$"Blog post with Id: {id} found";
            response.Data=data;
        }
        return response;
    }

    private static string UnescapeString(string escapedString)
    {
        return Regex.Unescape(escapedString);
    }
    public async Task<ServiceResponse<IEnumerable<BlogPost>>> Get()
    {
        var response = new ServiceResponse<IEnumerable<BlogPost>>();
        var data = await _appDbContext.BlogPosts.ToListAsync();
        data.ForEach(blog =>
        {
            _appDbContext.Entry(blog).Reference(b => b.Author).Load();

        });
        if(data is null)
        {
            response.Success=false;
            response.Message=$"Unable to retrieve the blog posts";
            response.Data=data;
        }
        else
        {
            data.ForEach(blog =>
            {
                blog.Content=string.Join(Environment.NewLine, blog.Content);
            });
            response.Success=true;
            response.Message=$"Blog posts retrieved";
            response.Data=data;
        }
        return response;
    }

    public async Task<ServiceResponse<BlogPost>> Update(BlogPost entity)
    {
        var response = new ServiceResponse<BlogPost>();
        var data = await _appDbContext.BlogPosts.FindAsync(entity.Id);
        if(data is null)
        {
            response.Success=false;
            response.Message=$"Unable to find the blog post with Title: {entity.Title}";
            response.Data=data;
        }
        else
        {
            data.Title=entity.Title;
            data.Content=entity.Content;
            data.Modified=DateTime.Now.ToUniversalTime();

            var rowsAffected = await _appDbContext.SaveChangesAsync();
            if(rowsAffected==1)
            {
                response.Success=true;
                response.Message=$"Blog post with Title: {entity.Title} updated succesfully";
                response.Data=data;
            }
            else
            {
                response.Success=false;
                response.Message=$"Unable to update blog post with Title: {entity.Title}";
                response.Data=data;
            }
        }
        return response;
    }
}
