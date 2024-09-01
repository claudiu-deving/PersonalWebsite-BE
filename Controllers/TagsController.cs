using ccsflowserver.Model;
using ccsflowserver.Services;

using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ccsflowserver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly IModelService<Tag> _tagService;

        public TagsController(IModelService<Tag> tagService)
        {
            _tagService = tagService;
        }

        // GET: api/<TagsController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDTO>>> Get()
        {
            try
            {
                var serviceResponse = await _tagService.Get();
                if (!serviceResponse.Success || serviceResponse.Data is null)
                {
                    return BadRequest(serviceResponse.Message);
                }
                else
                {
                    return Ok(serviceResponse.Data.Select(tag => new TagDTO(
                        tag.Id,
                        tag.Name,
                        tag.Color,
                        tag.Description,
                        tag.Blogs.Count)));
                }
            }
            catch
            {
                return BadRequest("An error occured, unable to process the request");
            }
        }

        // GET api/<TagsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TagDTO>> Get(int id)
        {
            try
            {
                var serviceResponse = await _tagService.Get(id);
                if (!serviceResponse.Success || serviceResponse.Data is null)
                {
                    return BadRequest(serviceResponse.Message);
                }
                else
                {
                    return Ok(new TagDTO(
                        serviceResponse.Data.Id,
                        serviceResponse.Data.Name,
                        serviceResponse.Data.Color,
                        serviceResponse.Data.Description,
                        serviceResponse.Data.Blogs.Count));
                }
            }
            catch
            {
                return BadRequest("An error occured, unable to process the request");
            }
        }

        // POST api/<TagsController>
        [HttpPost]
        public void Post([FromBody] TagDTO value)
        {
        }

        // PUT api/<TagsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TagsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
