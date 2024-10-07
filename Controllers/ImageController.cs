using ccsflowserver.Model;
using ccsflowserver.Services;
using Microsoft.AspNetCore.Mvc;

namespace ccsflowserver.Controllers
{
	public class ImageController : ControllerBase
	{
		public ImageController(IModelService<BlogPost> blogPostService)
		{
			var imageUploadDirectoryEnvVariable = Environment.GetEnvironmentVariable("PW_IMAGE_DIR")
				?? throw new Exception("No env var found for image directory");
			_uploadPath = imageUploadDirectoryEnvVariable;
			_blogPostService = blogPostService;
		}
		private readonly string _uploadPath = "";
		private readonly IModelService<BlogPost> _blogPostService;

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

		[HttpPost("api/upload-image-hero")]
		public async Task<IActionResult> UploadImageHero([FromForm] HeroImageUploadDto data)
		{
			var image = data.FormFile;
			var blogPostRetrieval = await _blogPostService.Get(data.BlogPostId);
			if (blogPostRetrieval is null) return BadRequest();
			if (!blogPostRetrieval.Success || blogPostRetrieval.Data is null) return NotFound($"{blogPostRetrieval.Message}");
			if (image == null || image.Length == 0) return BadRequest("No image file provided");
			if (string.IsNullOrEmpty(_uploadPath)) throw new Exception("No path provided for image directory");

			var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
			var filePath = Path.Combine(_uploadPath, fileName);
			blogPostRetrieval.Data.HeroImagePath = fileName;
			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await image.CopyToAsync(stream);
			}
			await _blogPostService.Update(blogPostRetrieval.Data);
			return Ok(new { filename = fileName });
		}
	}

	public record HeroImageUploadDto(IFormFile FormFile, int BlogPostId)
	{
		public IFormFile FormFile { get; } = FormFile;
		public int BlogPostId { get; } = BlogPostId;
	}
}
