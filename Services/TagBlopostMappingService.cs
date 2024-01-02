using ccsflowserver.Data;
using ccsflowserver.Model;

using Microsoft.EntityFrameworkCore;

namespace ccsflowserver.Services;


public class TagBlopostMappingService : IModelService<TagBlogpostMapping>
{
    private readonly AppDbContext _appDbContext;

    public TagBlopostMappingService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }
    public async Task<ServiceResponse<TagBlogpostMapping>> Create(TagBlogpostMapping entity)
    {
        ServiceResponse<TagBlogpostMapping> serviceResponse = new();
        var existingTagBlogpostMappings = await _appDbContext.TagBlogpostMappings.ToListAsync();
        if (existingTagBlogpostMappings.Select(x => x.Id).Contains(entity.Id))
        {
            serviceResponse.Success = false;
            serviceResponse.Message = "A TagBlogpostMapping with the same name already exists";
            serviceResponse.Data = existingTagBlogpostMappings.First(x => x.Id.Equals(entity.Id));
            return serviceResponse;
        }
        _appDbContext.TagBlogpostMappings.Add(entity);
        var rowsAffected = await _appDbContext.SaveChangesAsync();

        if (rowsAffected == 0)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = "Unable to save the TagBlogpostMapping into the database";
            serviceResponse.Data = null;
            return serviceResponse;
        }
        existingTagBlogpostMappings = await _appDbContext.TagBlogpostMappings.ToListAsync();

        serviceResponse.Success = true;
        serviceResponse.Message = "TagBlogpostMapping succesfully created";
        serviceResponse.Data = existingTagBlogpostMappings.First(x => x.Id.Equals(entity.Id));
        return serviceResponse;
    }

    public async Task<ServiceResponse<bool>> Delete(object id)
    {
        ServiceResponse<bool> serviceResponse = new();

        var existingTagBlogpostMappings = await _appDbContext.TagBlogpostMappings.FindAsync(id);
        if (existingTagBlogpostMappings is null)
        {
            serviceResponse.Success = true;
            serviceResponse.Message = "The TagBlogpostMapping doesn't exist";
            serviceResponse.Data = true;
            return serviceResponse;
        }
        _appDbContext.TagBlogpostMappings.Remove(existingTagBlogpostMappings);
        var rowsAffected = await _appDbContext.SaveChangesAsync();

        if (rowsAffected == 0)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = "Unable to remove the TagBlogpostMapping from the database";
            serviceResponse.Data = false;
            return serviceResponse;
        }
        serviceResponse.Success = true;
        serviceResponse.Message = "TagBlogpostMapping succesfully removed";
        return serviceResponse;
    }

    public async Task<ServiceResponse<TagBlogpostMapping>> Get(object id)
    {
        ServiceResponse<TagBlogpostMapping> serviceResponse = new();

        var existingTagBlogpostMapping = await _appDbContext.TagBlogpostMappings.FindAsync(id);
        if (existingTagBlogpostMapping is null)
        {
            serviceResponse.Success = true;
            serviceResponse.Message = "The TagBlogpostMapping doesn't exist";
            serviceResponse.Data = null;
            return serviceResponse;
        }
        serviceResponse.Success = true;
        serviceResponse.Message = "TagBlogpostMapping succesfully retrieved";
        serviceResponse.Data = existingTagBlogpostMapping;
        return serviceResponse;
    }

    public async Task<ServiceResponse<IEnumerable<TagBlogpostMapping>>> Get()
    {
        ServiceResponse<IEnumerable<TagBlogpostMapping>> serviceResponse = new();

        var existingTagBlogpostMappings = await _appDbContext.TagBlogpostMappings.ToListAsync();
        if (existingTagBlogpostMappings is null)
        {
            serviceResponse.Success = true;
            serviceResponse.Message = "No TagBlogpostMappings found";
            serviceResponse.Data = null;
            return serviceResponse;
        }
        serviceResponse.Success = true;
        serviceResponse.Message = "TagBlogpostMapping succesfully retrieved";
        serviceResponse.Data = existingTagBlogpostMappings;
        return serviceResponse;
    }

    public async Task<ServiceResponse<TagBlogpostMapping>> Update(TagBlogpostMapping entity)
    {
        ServiceResponse<TagBlogpostMapping> serviceResponse = new();
        var existingTagBlogpostMapping = await _appDbContext.TagBlogpostMappings.FindAsync(entity.Id);
        if (existingTagBlogpostMapping is null)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = "The TagBlogpostMapping doesn't exist";
            serviceResponse.Data = null;
            return serviceResponse;
        }
        _appDbContext.TagBlogpostMappings.Update(existingTagBlogpostMapping);
        var rowsAffected = await _appDbContext.SaveChangesAsync();
        if (rowsAffected == 0)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = "Unable to update the TagBlogpostMapping into the database";
            serviceResponse.Data = null;
            return serviceResponse;
        }

        serviceResponse.Success = true;
        serviceResponse.Message = "TagBlogpostMapping succesfully created";
        serviceResponse.Data = entity;
        return serviceResponse;
    }
}



