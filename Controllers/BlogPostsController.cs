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
    private readonly IMapper _autoMapper;

    public BlogPostsController(IModelService<BlogPost> blogpostsService, IAuthservice authservice, IModelService<User> userService)
    {
        _blogpostsService=blogpostsService;
        _authservice=authservice;
        _userService=userService;
        _autoMapper=new AutoMapper.MapperConfiguration(cfg =>
        {
            cfg.CreateMap<BlogPostUpdate, BlogPost>()
               .ForAllMembers(opts => opts.Condition((src, dest, srcMember, destMember, context) =>
               {
                   // Check if the source member is null
                   return srcMember!=null;
               }));
        }).CreateMapper();

    }

    // GET: api/<BlogPostsController>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BlogPostUpdate>>> Get()
    {
        var data = await _blogpostsService.Get();
        if(data.Success&&data.Data is not null)
        {
            return Ok(data.Data);
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
    // GET api/<BlogPostsController>/5
    [HttpGet("unparsed/{id}")]
    public async Task<ActionResult<BlogPost>> GetUnparsed(int id)
    {
        var data = await _blogpostsService.Get(id, false);
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
    public async Task<ActionResult> Post([FromBody] BlogPost blogPost, string userName)
    {
        if(_authservice.UserExists(userName).Result==false)
        {
            return Unauthorized();
        }

        if(blogPost.Author is null||!blogPost.Author.Username.Equals(userName))
        {
            return Unauthorized();
        }


        if(blogPost==null)
        {
            return BadRequest("The object cannot be null");
        }
        if(string.IsNullOrEmpty(blogPost.Title))
        {
            return BadRequest("Title is required");
        }
        if(string.IsNullOrEmpty(blogPost.Content))
        {
            return BadRequest("Content is required");
        }
        var data = await _blogpostsService.Create(blogPost);
        if(data.Success)
        {
            return Ok(data.Data);
        }
        else
        {
            return BadRequest(data.Message);
        }
    }

    // PUT api/<BlogPostsController>/5
    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, [FromBody] BlogPostUpdate blogPost)
    {
        var existing = await _blogpostsService.Get(id);
        if(existing.Success)
        {
            _autoMapper.Map(blogPost, existing.Data);
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
            return NotFound($"Blog post with Id {id} was not found");
        }
    }
}
