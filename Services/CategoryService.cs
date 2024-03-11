using ccsflowserver.Data;
using ccsflowserver.Model;

using Microsoft.EntityFrameworkCore;

namespace ccsflowserver.Services;

public class CategoryService : IModelService<Category>
{
    private readonly AppDbContext _appDbContext;

    public CategoryService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }
    public async Task<ServiceResponse<Category>> Create(Category entity)
    {
        ServiceResponse<Category> serviceResponse = new();
        var existingCategories = await _appDbContext.Categories.ToListAsync();
        if (existingCategories.Select(x => x.Name).Contains(entity.Name))
        {
            serviceResponse.Success = false;
            serviceResponse.Message = "A category with the same name already exists";
            serviceResponse.Data = existingCategories.First(x => x.Name.Equals(entity.Name));
            return serviceResponse;
        }
        _appDbContext.Categories.Add(entity);
        var rowsAffected = await _appDbContext.SaveChangesAsync();

        if (rowsAffected == 0)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = "Unable to save the category into the database";
            serviceResponse.Data = null;
            return serviceResponse;
        }
        existingCategories = await _appDbContext.Categories.ToListAsync();

        serviceResponse.Success = true;
        serviceResponse.Message = "Category succesfully created";
        serviceResponse.Data = existingCategories.First(x => x.Name.Equals(entity.Name));
        return serviceResponse;
    }

    public async Task<ServiceResponse<bool>> Delete(object id)
    {
        ServiceResponse<bool> serviceResponse = new();

        var existingCategories = await _appDbContext.Categories.FindAsync(id);
        if (existingCategories is null)
        {
            serviceResponse.Success = true;
            serviceResponse.Message = "The category doesn't exist";
            serviceResponse.Data = true;
            return serviceResponse;
        }
        _appDbContext.Categories.Remove(existingCategories);
        var rowsAffected = await _appDbContext.SaveChangesAsync();

        if (rowsAffected == 0)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = "Unable to remove the category from the database";
            serviceResponse.Data = false;
            return serviceResponse;
        }
        serviceResponse.Success = true;
        serviceResponse.Message = "Category succesfully removed";
        return serviceResponse;
    }

    public async Task<ServiceResponse<Category>> Get(object id)
    {
        ServiceResponse<Category> serviceResponse = new();

        var existingCategory = await _appDbContext.Categories.FindAsync(id);
        if (existingCategory is null)
        {
            serviceResponse.Success = true;
            serviceResponse.Message = "The category doesn't exist";
            serviceResponse.Data = null;
            return serviceResponse;
        }
        serviceResponse.Success = true;
        serviceResponse.Message = "Category succesfully retrieved";
        serviceResponse.Data = existingCategory;
        return serviceResponse;
    }

    public async Task<ServiceResponse<IEnumerable<Category>>> Get()
    {
        ServiceResponse<IEnumerable<Category>> serviceResponse = new();

        var existingCategories = await _appDbContext.Categories.ToListAsync();
        if (existingCategories is null)
        {
            serviceResponse.Success = true;
            serviceResponse.Message = "No categories found";
            serviceResponse.Data = null;
            return serviceResponse;
        }
        serviceResponse.Success = true;
        serviceResponse.Message = "Category succesfully retrieved";
        serviceResponse.Data = existingCategories;
        return serviceResponse;
    }

    public async Task<ServiceResponse<Category>> Update(Category entity)
    {
        ServiceResponse<Category> serviceResponse = new();
        var existingCategory = await _appDbContext.Categories.FindAsync(entity.Id);
        if (existingCategory is null)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = "The category doesn't exist";
            serviceResponse.Data = null;
            return serviceResponse;
        }
        _appDbContext.Categories.Update(existingCategory);
        var rowsAffected = await _appDbContext.SaveChangesAsync();
        if (rowsAffected == 0)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = "Unable to update the category into the database";
            serviceResponse.Data = null;
            return serviceResponse;
        }

        serviceResponse.Success = true;
        serviceResponse.Message = "Category succesfully created";
        serviceResponse.Data = entity;
        return serviceResponse;
    }
}
