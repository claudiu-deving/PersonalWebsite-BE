using System.Threading.Tasks;

using AutoMapper;

using ccsflowserver.Model;
using ccsflowserver.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ccsflowserver.Controllers;
[Route("api/[controller]")]
[ApiController]
public class BlogPostsController : ControllerBase
{
    private readonly IModelService<BlogPost> _blogpostsService;
    private readonly IAuthservice _authservice;
    private readonly IModelService<User> _userService;


    public BlogPostsController(IModelService<BlogPost> blogpostsService, IAuthservice authservice, IModelService<User> userService)
    {
        _blogpostsService=blogpostsService;
        _authservice=authservice;
        _userService=userService;

    }

    // GET: api/<BlogPostsController>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BlogPostUpdate>>> Get()
    {
        var data = await _blogpostsService.Get();
        var roles = await _userService.Get();
        List<BlogPostUpdate> blogs = new();
        if(data.Success&&data.Data is not null)
        {
            foreach(var blog in data.Data)
            {

                blogs.Add(new BlogPostUpdate()
                {
                    Id=blog.Id,
                    Title=blog.Title,
                    Content=blog.Content,
                    Created=blog.Created,
                    Modified=blog.Modified,
                    IsApproved=blog.IsApproved,
                    Author=new UserPayload()
                    {
                        Id=blog.Author.Id,
                        Username=blog.Author.Username,
                        IsAdmin=blog.Author.Role.IsAdmin
                    }
                });

            }
            return Ok(blogs);
        }
        else
        {
            return NotFound(data.Message);
        }
    }

    // GET api/<BlogPostsController>/5
    [HttpGet("{id}")]
    public async Task<ActionResult<BlogPost>> Get(int id)
    {
        var data = await _blogpostsService.Get(id);
        if(data.Success&&data.Data is not null)
        {
            return Ok(data.Data);
        }
        else
        {
            return NotFound(data.Message);
        }
    }


    // POST api/<BlogPostsController>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult> Post([FromBody] BlogPostCreate requestBlogPost)
    {
        var guid = new Guid(requestBlogPost.UserId);
        if(_authservice.UserExists(guid).Result==false)
        {
            return Unauthorized();
        }

        if(guid==Guid.Empty)
        {
            return Unauthorized();
        }
        if(requestBlogPost==null)
        {
            return BadRequest("The object cannot be null");
        }
        if(string.IsNullOrEmpty(requestBlogPost.Title))
        {
            return BadRequest("Title is required");
        }
        if(string.IsNullOrEmpty(requestBlogPost.Content))
        {
            return BadRequest("Content is required");
        }
        var response = await _userService.Get(guid);
        if(!response.Success)
        {
            return NotFound(response.Message);
        }
        if(response.Data is null)
        {
            return NotFound(response.Message);
        }
        var author = response.Data;
        BlogPost blogPost = new BlogPost(requestBlogPost.Title, requestBlogPost.Content, author);


        var data = await _blogpostsService.Create(blogPost);

        var responseBlogPost = new BlogPostUpdate()
        {
            Title=data.Data.Title,
            Content=data.Data.Content,
            Author=new UserPayload()
            {
                Username=data.Data.Author.Username,
                Id=data.Data.Author.Id,
                IsAdmin=data.Data.Author.Role.IsAdmin
            },
            Id=data.Data.Id,


        };
        if(data.Success)
        {
            return Ok(responseBlogPost);
        }
        else
        {
            return BadRequest(data.Message);
        }
    }


    // PUT api/<BlogPostsController>/5
    [HttpPut("approve/{id}")]
    [Authorize]
    public async Task<ActionResult> SetApproval(int id, [FromBody] bool approved)
    {
        var existing = await _blogpostsService.Get(id, false);
        if(existing.Success&&existing.Data is not null)
        {
            existing.Data.IsApproved=approved;
        }
        else
        {
            return NotFound(existing.Message);
        }

        var data = await _blogpostsService.Update(existing.Data);
        if(data.Success)
        {
            return Ok();
        }
        else
        {
            return NotFound(data.Message);
        }

    }

    // PUT api/<BlogPostsController>/5
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult> Put(int id, [FromBody] BlogPostUpdate blogPost)
    {
        var existing = await _blogpostsService.Get(id, false);
        if(existing.Success)
        {
            existing.Data.Content=blogPost.Content;
            existing.Data.Title=blogPost.Title;
            existing.Data.Modified=DateTime.Now.ToUniversalTime();
        }
        else
        {
            var author = await _userService.Get(blogPost.Author.Id);
            if(!author.Success)
            {
                return NotFound(author.Message);
            }
            var blogPostNew = new BlogPost(blogPost.Title, blogPost.Content, author.Data);
            _blogpostsService.Create(blogPostNew);
        }

        var data = await _blogpostsService.Update(existing.Data);
        if(data.Success)
        {
            return Ok();
        }
        else
        {
            return NotFound(data.Message);
        }

    }

    // DELETE api/<BlogPostsController>/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var existing = await _blogpostsService.Get(id);
        if(existing.Success)
        {
            await _blogpostsService.Delete(id);
            return Ok();
        }
        else
        {
            return Ok();
        }
    }
}
