using Microsoft.AspNetCore.Mvc;

namespace ccsflowserver.Controllers
{
	public class ImageController : ControllerBase
	{
		public ImageController()
		{
			var imageUploadDirectoryEnvVariable = Environment.GetEnvironmentVariable("PW_IMAGE_DIR")
				?? throw new Exception("No env var found for image directory");
			_uploadPath = imageUploadDirectoryEnvVariable;
		}
		private readonly string _uploadPath = "";
		[HttpPost("api/upload-image")]
		public async Task<IActionResult> UploadImage(IFormFile image)
		{
			if (image == null || image.Length == 0) return BadRequest("No image file provided");
			if (string.IsNullOrEmpty(_uploadPath)) throw new Exception("No path provided for image directory");

			var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
			var filePath = Path.Combine(_uploadPath, fileName);

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await image.CopyToAsync(stream);
			}

			return Ok(new { filename = fileName });
		}
	}
}
