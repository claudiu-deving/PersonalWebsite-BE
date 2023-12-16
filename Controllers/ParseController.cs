
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace ccsflowserver.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ParseController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<string>> Parse([FromBody] string content)
    {
        try
        {
            var escapedForJson = await Task.Run(() =>
             {
                 return JsonConvert.SerializeObject("");
             });

            return Ok(escapedForJson);
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
