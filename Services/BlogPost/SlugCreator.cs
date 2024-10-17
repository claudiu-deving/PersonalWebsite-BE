using ccsflowserver.Model;
using Google.Apis.Http;

namespace ccsflowserver.Services;

public class SlugCreator(IModelService<BlogPost> blogPostService)
{

	/*
	this-is-title
	this-is-title-1
	next-title

	This is title

	Next title

	New title

	 */

	public async Task<string> CreateSlug(string title)
	{
		var existingSlugsServiceRequest = await blogPostService.Get();
		var existingSlugs = new List<string>();
		var slug = title.ToLower().Trim().Replace(" ", "-");
		int maxIndex = 0;
		if (existingSlugsServiceRequest.Success)
		{
			existingSlugs = existingSlugsServiceRequest.Data!.Where(x => x.Slug.Contains(slug)).Select(x => x.Slug).ToList();
			if (existingSlugs.Count != 0)
			{
				maxIndex = existingSlugs.Select(ExtractIndexFromSlug).ToList().Max();
			}
			else
			{
				maxIndex = -1;
			}
		}

		if (maxIndex != -1)
		{
			maxIndex++;
			slug += $"-{maxIndex}";
		}

		return slug;
	}

	private static int ExtractIndexFromSlug(string temporarySlug)
	{
		if (int.TryParse(temporarySlug.Last().ToString(), out int parsedIndex))
		{
			return parsedIndex;
		}
		else
		{
			return 0;
		}
	}
}