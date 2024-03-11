using ccsflowserver.Data;
using ccsflowserver.Model;

using Microsoft.EntityFrameworkCore;

namespace ccsflowserver.Services;


public class TagService : IModelService<Tag>
{
    private readonly AppDbContext _appDbContext;

    public TagService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }
    public async Task<ServiceResponse<Tag>> Create(Tag entity)
    {
        ServiceResponse<Tag> serviceResponse = new();
        var existingTags = await _appDbContext.Tags.ToListAsync();
        if (existingTags.Select(x => x.Name).Contains(entity.Name))
        {
            serviceResponse.Success = false;
            serviceResponse.Message = "A Tag with the same name already exists";
            serviceResponse.Data = existingTags.First(x => x.Name.Equals(entity.Name));
            return serviceResponse;
        }
        _appDbContext.Tags.Add(entity);
        var rowsAffected = await _appDbContext.SaveChangesAsync();

        if (rowsAffected == 0)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = "Unable to save the Tag into the database";
            serviceResponse.Data = null;
            return serviceResponse;
        }
        existingTags = await _appDbContext.Tags.ToListAsync();

        serviceResponse.Success = true;
        serviceResponse.Message = "Tag succesfully created";
        serviceResponse.Data = existingTags.First(x => x.Name.Equals(entity.Name));
        return serviceResponse;
    }

    public async Task<ServiceResponse<bool>> Delete(object id)
    {
        ServiceResponse<bool> serviceResponse = new();

        var existingTags = await _appDbContext.Tags.FindAsync(id);
        if (existingTags is null)
        {
            serviceResponse.Success = true;
            serviceResponse.Message = "The Tag doesn't exist";
            serviceResponse.Data = true;
            return serviceResponse;
        }
        _appDbContext.Tags.Remove(existingTags);
        var rowsAffected = await _appDbContext.SaveChangesAsync();

        if (rowsAffected == 0)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = "Unable to remove the Tag from the database";
            serviceResponse.Data = false;
            return serviceResponse;
        }
        serviceResponse.Success = true;
        serviceResponse.Message = "Tag succesfully removed";
        return serviceResponse;
    }

    public async Task<ServiceResponse<Tag>> Get(object id)
    {
        ServiceResponse<Tag> serviceResponse = new();

        var existingTag = await _appDbContext.Tags.FindAsync(id);
        if (existingTag is null)
        {
            serviceResponse.Success = true;
            serviceResponse.Message = "The Tag doesn't exist";
            serviceResponse.Data = null;
            return serviceResponse;
        }
        serviceResponse.Success = true;
        serviceResponse.Message = "Tag succesfully retrieved";
        serviceResponse.Data = existingTag;
        return serviceResponse;
    }

    public async Task<ServiceResponse<IEnumerable<Tag>>> Get()
    {
        ServiceResponse<IEnumerable<Tag>> serviceResponse = new();

        var existingTags = await _appDbContext.Tags.ToListAsync();
        if (existingTags is null)
        {
            serviceResponse.Success = true;
            serviceResponse.Message = "No Tags found";
            serviceResponse.Data = null;
            return serviceResponse;
        }
        serviceResponse.Success = true;
        serviceResponse.Message = "Tag succesfully retrieved";
        serviceResponse.Data = existingTags;
        return serviceResponse;
    }

    public async Task<ServiceResponse<Tag>> Update(Tag entity)
    {
        ServiceResponse<Tag> serviceResponse = new();
        var existingTag = await _appDbContext.Tags.FindAsync(entity.Id);
        if (existingTag is null)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = "The Tag doesn't exist";
            serviceResponse.Data = null;
            return serviceResponse;
        }
        _appDbContext.Tags.Update(existingTag);
        var rowsAffected = await _appDbContext.SaveChangesAsync();
        if (rowsAffected == 0)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = "Unable to update the Tag into the database";
            serviceResponse.Data = null;
            return serviceResponse;
        }

        serviceResponse.Success = true;
        serviceResponse.Message = "Tag succesfully created";
        serviceResponse.Data = entity;
        return serviceResponse;
    }
}



