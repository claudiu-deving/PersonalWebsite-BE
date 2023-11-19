using System.Text.RegularExpressions;

using MdToHtml;

using Microsoft.AspNetCore.Http;
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
                 var parsedLines = Parser.Process(content);
                 var parsedContent = string.Join(Environment.NewLine, parsedLines);
                 return JsonConvert.SerializeObject(parsedContent);
             });

            return Ok(escapedForJson);
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
