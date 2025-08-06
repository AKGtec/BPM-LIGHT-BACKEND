using ProjectTemplate.Shared.RequestFeatures;

namespace ProjectTemplate.Service.Contracts;

public interface IBaseService<TDto, TEntity>
{
    Task<IEnumerable<TDto>> GetAllAsync(RequestParameters parameters, bool trackChanges);
    Task<TDto?> GetByIdAsync(Guid id, bool trackChanges);
    Task<TDto> CreateAsync(TDto dto);
    Task UpdateAsync(Guid id, TDto dto, bool trackChanges);
    Task DeleteAsync(Guid id, bool trackChanges);
}
