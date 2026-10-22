using AutoMapper;
using StoreServer.models;
using StoreServer.models.DTO;
using StoreServer.Repositories;

namespace StoreServer.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<(List<CategoryDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct)
        {
            if (page < 1 || pageSize < 1 || (long)(page - 1) * pageSize > int.MaxValue)
            {
                throw new InvalidOperationException("Некорректные параметры пагинации.");
            }

            var items = await _repository.GetAllAsync(page, pageSize, ct);
            var total = await _repository.CountAsync(ct);
            return (_mapper.Map<List<CategoryDTO>>(items), total);
        }

        public async Task<CategoryDTO?> GetByIdAsync(int id, CancellationToken ct)
        {
            var category = await _repository.GetByIdAsync(id, ct);
            return category == null ? null : _mapper.Map<CategoryDTO>(category);
        }

        public async Task<CategoryDTO> CreateAsync(CreateCategoryDTO dto, CancellationToken ct)
        {
            var category = _mapper.Map<Category>(dto);
            await _repository.AddAsync(category, ct);
            await _repository.SaveChangesAsync(ct);
            category = await _repository.GetByIdAsync(category.IdCategory, ct)
                ?? throw new InvalidOperationException("Не удалось загрузить созданную запись.");
            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task<CategoryDTO?> UpdateAsync(int id, UpdateCategoryDTO dto, CancellationToken ct)
        {
            var category = await _repository.GetByIdAsync(id, ct);
            if (category == null)
            {
                return null;
            }

            _mapper.Map(dto, category);
            _repository.Update(category);
            await _repository.SaveChangesAsync(ct);
            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task<CategoryDTO?> PatchAsync(int id, PatchCategoryDTO dto, CancellationToken ct)
        {
            var category = await _repository.GetByIdAsync(id, ct);
            if (category == null)
            {
                return null;
            }

            if (dto.CategoryName != null)
            {
                category.CategoryName = dto.CategoryName;
            }
            if (dto.CategoryDescription != null)
            {
                category.CategoryDescription = dto.CategoryDescription;
            }

            _repository.Update(category);
            await _repository.SaveChangesAsync(ct);
            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var category = await _repository.GetByIdAsync(id, ct);
            if (category == null)
            {
                return false;
            }

            _repository.Remove(category);
            await _repository.SaveChangesAsync(ct);
            return true;
        }
    }
}
