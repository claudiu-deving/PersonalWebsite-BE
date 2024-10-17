using Microsoft.AspNetCore.Mvc;

namespace ccsflowserver.Controllers
{
	public class VideoController : Controller
	{
		private readonly string _uploadPath;

		public VideoController()
		{
			var imageUploadDirectoryEnvVariable = Environment.GetEnvironmentVariable("PW_IMAGE_DIR")
		?? throw new Exception("No env var found for image directory");
			_uploadPath = imageUploadDirectoryEnvVariable;
		}

		[HttpPost("api/upload-video")]
		public async Task<IActionResult> UploadVideo(IFormFile video, [FromForm] int blogId)
		{
			if (video == null || video.Length == 0)
				return BadRequest("No video file provided");

			if (string.IsNullOrEmpty(_uploadPath))
				throw new Exception("No path provided for video directory");

			var fileName = $"{Guid.NewGuid()}{Path.GetExtension(video.FileName)}";
			var filePath = Path.Combine(_uploadPath, fileName);

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await video.CopyToAsync(stream);
			}

			// Here you might want to save the video information to your database
			// For example: await _dbContext.Videos.AddAsync(new Video { FileName = fileName, BlogId = blogId });
			// await _dbContext.SaveChangesAsync();

			return Ok(new { fileName = fileName, filePath = $"/assets/blogs/{fileName}" });
		}
	}
}
