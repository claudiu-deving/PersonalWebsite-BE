namespace ccsflowserver.Services;

public interface IModelService<T> where T : IEntity
{
    Task<ServiceResponse<T>> Create(T entity);
    Task<ServiceResponse<T>> Update(T entity);
    Task<ServiceResponse<bool>> Delete(object id);
    Task<ServiceResponse<T>> Get(object id);
    Task<ServiceResponse<IEnumerable<T>>> Get();
}

