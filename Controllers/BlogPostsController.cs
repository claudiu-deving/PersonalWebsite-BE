using System.Security.Claims;
using System.Threading.Tasks;

using AutoMapper;

using ccsflowserver.Model;
using ccsflowserver.Services;

using Microsoft.AspNetCore.Authentication;
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
    private readonly IClaimsTranslator _claimsTransformation;

    public BlogPostsController(
        IModelService<BlogPost> blogpostsService,
        IAuthservice authservice,
        IModelService<User> userService,
        IClaimsTranslator claimsTransformation)
    {
        _blogpostsService=blogpostsService;
        _authservice=authservice;
        _userService=userService;
        _claimsTransformation=claimsTransformation;
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


        if(data.Success&&data.Data is BlogPost blog)
        {
            return Ok(new BlogPostUpdate()
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

        var userId = _claimsTransformation.GetUserId(HttpContext.User);

        if(userId==Guid.Empty)
        {
            return Unauthorized("Username not found in token.");
        }

        if(_authservice.UserExists(userId).Result==false)
        {
            return Unauthorized("The user doesn't exist");
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
        var response = await _userService.Get(userId);
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
        var isAdmin = _claimsTransformation.IsAdmin(HttpContext.User);
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
        var username = _claimsTransformation.GetUserName(HttpContext.User);

        // Check if the userId is null or empty
        if(string.IsNullOrEmpty(username))
        {
            return Unauthorized("Username not found in token.");
        }
        var serviceResponse = await _blogpostsService.Get(id, false);
        if(serviceResponse.Success&&serviceResponse.Data is BlogPost existingBlog)
        {

            if(!username.Equals(existingBlog.Author.Username))
            {
                return Unauthorized("You are not authorized to update this blog post");
            }
            existingBlog.Content=blogPost.Content;
            existingBlog.Title=blogPost.Title;
            existingBlog.Modified=DateTime.Now.ToUniversalTime();
            var data = await _blogpostsService.Update(existingBlog);
            if(data.Success)
            {
                return Ok();
            }
            else
            {
                return NotFound(data.Message);
            }
        }
        else
        {
            var author = await _userService.Get(blogPost.Author.Id);
            if(!author.Success)
            {
                return NotFound(author.Message);
            }
            var blogPostNew = new BlogPost(blogPost.Title, blogPost.Content, author.Data);
            var blogpostCreation = await _blogpostsService.Create(blogPostNew);
            if(blogpostCreation.Success)
            {
                return Ok();
            }
            else
            {
                return NotFound(blogpostCreation.Message);
            }
        }



    }

    // DELETE api/<BlogPostsController>/5
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Delete(int id)
    {
        var isAdmin = _claimsTransformation.IsAdmin(User);
        var userName = _claimsTransformation.GetUserName(User);
        if(userName is null)
        {
            return Unauthorized("You are not authorized to perform this action, you must be either the author or an admin");
        }
        var existing = await _blogpostsService.Get(id);
        if(existing.Data is not null&&(isAdmin||userName.Equals(existing.Data.Author.Username))&&existing.Success)
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
