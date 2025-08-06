using AutoMapper;
using ProjectTemplate.Contracts;
using ProjectTemplate.Contracts.Repository;
using ProjectTemplate.Models.Entities;
using ProjectTemplate.Models.Exceptions;
using ProjectTemplate.Service.Contracts;
using ProjectTemplate.Shared.RequestFeatures;

namespace ProjectTemplate.Service;

public abstract class BaseService<TDto, TEntity> : IBaseService<TDto, TEntity> 
    where TEntity : BaseEntity 
    where TDto : class
{
    protected readonly IRepositoryManager _repository;
    protected readonly ILoggerManager _logger;
    protected readonly IMapper _mapper;

    public BaseService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public virtual async Task<IEnumerable<TDto>> GetAllAsync(RequestParameters parameters, bool trackChanges)
    {
        // Override this method in derived classes to implement specific logic
        throw new NotImplementedException("GetAllAsync must be implemented in derived class");
    }

    public virtual async Task<TDto?> GetByIdAsync(Guid id, bool trackChanges)
    {
        // Override this method in derived classes to implement specific logic
        throw new NotImplementedException("GetByIdAsync must be implemented in derived class");
    }

    public virtual async Task<TDto> CreateAsync(TDto dto)
    {
        // Override this method in derived classes to implement specific logic
        throw new NotImplementedException("CreateAsync must be implemented in derived class");
    }

    public virtual async Task UpdateAsync(Guid id, TDto dto, bool trackChanges)
    {
        // Override this method in derived classes to implement specific logic
        throw new NotImplementedException("UpdateAsync must be implemented in derived class");
    }

    public virtual async Task DeleteAsync(Guid id, bool trackChanges)
    {
        // Override this method in derived classes to implement specific logic
        throw new NotImplementedException("DeleteAsync must be implemented in derived class");
    }

    protected void CheckIfEntityExists(TEntity? entity, Guid id)
    {
        if (entity is null)
            throw new EntityNotFoundException(typeof(TEntity).Name, id);
    }
}
