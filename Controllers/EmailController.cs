using ccsflowserver.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Net;
using System.Net.Mail;

namespace ccsflowserver.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EmailController : ControllerBase
	{
		private readonly IConfiguration _configuration;

		public EmailController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[HttpPost("send")]
		public IActionResult SendEmail([FromBody] Email model)
		{
			try
			{
				var smtpUsername = Environment.GetEnvironmentVariable("SMTP_USERNAME");
				var smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD");
				var recipientEmail = Environment.GetEnvironmentVariable("RECIPIENT_EMAIL");

				if (string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpPassword) || string.IsNullOrEmpty(recipientEmail))
				{
					return StatusCode(500, "SMTP credentials or recipient email not configured.");
				}

				var smtpClient = new SmtpClient("smtp.gmail.com")
				{
					Port = 587,
					Credentials = new NetworkCredential(smtpUsername, smtpPassword),
					EnableSsl = true,
				};

				var mailMessage = new MailMessage
				{
					From = new MailAddress(model.SenderEmail, model.SenderName),
					Subject = model.Subject,
					Body = $@"<p>You have received a message on bitluz.com from:{model.SenderName} </p><h3><a href=""{model.SenderEmail}""></a></h3><p> With the message:<p>
							<div>{model.Message}</div>",
					IsBodyHtml = true,
					Sender = new MailAddress(model.SenderEmail, model.SenderName),

				};

				mailMessage.To.Add(recipientEmail);
				mailMessage.ReplyToList.Add(model.SenderEmail);
				smtpClient.Send(mailMessage);

				return StatusCode(200, "Email sent successfully");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred: {ex.Message}");
			}
		}
	}
}
