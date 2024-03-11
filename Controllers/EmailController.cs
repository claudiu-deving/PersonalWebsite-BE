using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using NuGet.Configuration;

using System.Net;
using System.Net.Mail;

namespace ccsflowserver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        [HttpPost]
        public IActionResult SendEmail([FromBody] EmailRequest request)
        {
            try
            {
                var username = Environment.GetEnvironmentVariable("EMAIL_ADDRESS");
                var password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
                var emailHost = Environment.GetEnvironmentVariable("EMAIL_HOST");
                if (username is null || password is null)
                {
                    return BadRequest("Unable to retrieve the e-mail credentials");
                }
                Console.WriteLine($"Trying with: {username} {password}");
                var email = new MailMessage();
                var client = new SmtpClient();
                email.From = new MailAddress("test@test.com");
#if RELEASE
                email.From = new MailAddress(username);
#endif


                email.To.Add("claudiu.s.dev@outlook.com");
                email.Subject = request.Subject ?? "";
                var formattedBody = request.Body.Replace("\n", "<br>");
                email.Body = $"<p>Someone contacted you on your website:</p><p>{request.From} says:</p><p>{formattedBody}</p><p>{request.Name}</p>";
                email.IsBodyHtml = true;
                client.Host = emailHost;
                client.Port = 587;
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(username, password);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(email);

                return Ok(new { message = "E-mail sent successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error");
            }
        }
    }

    public class EmailRequest
    {
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Name { get; set; }
    }
}
