using AutoMapper;
using StoreServer.models;
using StoreServer.models.DTO;
using StoreServer.Repositories;

namespace StoreServer.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<(List<ProductDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct)
        {
            if (page < 1 || pageSize < 1 || (long)(page - 1) * pageSize > int.MaxValue)
            {
                throw new InvalidOperationException("Некорректные параметры пагинации.");
            }

            var items = await _repository.GetAllAsync(page, pageSize, ct);
            var total = await _repository.CountAsync(ct);
            return (_mapper.Map<List<ProductDTO>>(items), total);
        }

        public async Task<ProductDTO?> GetByIdAsync(int id, CancellationToken ct)
        {
            var product = await _repository.GetByIdAsync(id, ct);
            return product == null ? null : _mapper.Map<ProductDTO>(product);
        }

        public async Task<ProductDTO> CreateAsync(CreateProductDTO dto, CancellationToken ct)
        {
            await ValidateReferencesAsync(dto.IdCategory, ct);
            var product = _mapper.Map<Product>(dto);
            await _repository.AddAsync(product, ct);
            await _repository.SaveChangesAsync(ct);
            product = await _repository.GetByIdAsync(product.IdProduct, ct)
                ?? throw new InvalidOperationException("Не удалось загрузить созданную запись.");
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<ProductDTO?> UpdateAsync(int id, UpdateProductDTO dto, CancellationToken ct)
        {
            var product = await _repository.GetByIdAsync(id, ct);
            if (product == null)
            {
                return null;
            }

            await ValidateReferencesAsync(dto.IdCategory, ct);
            _mapper.Map(dto, product);
            _repository.Update(product);
            await _repository.SaveChangesAsync(ct);
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<ProductDTO?> PatchAsync(int id, PatchProductDTO dto, CancellationToken ct)
        {
            var product = await _repository.GetByIdAsync(id, ct);
            if (product == null)
            {
                return null;
            }

            await ValidateReferencesAsync(dto.IdCategory, ct);
            if (dto.AvailableQuantity != null)
            {
                product.AvailableQuantity = dto.AvailableQuantity.Value;
            }
            if (dto.ProductName != null)
            {
                product.ProductName = dto.ProductName;
            }
            if (dto.ProductDescription != null)
            {
                product.ProductDescription = dto.ProductDescription;
            }
            if (dto.Price != null)
            {
                product.Price = dto.Price.Value;
            }
            if (dto.IdCategory != null)
            {
                product.IdCategory = dto.IdCategory.Value;
            }


            _repository.Update(product);
            await _repository.SaveChangesAsync(ct);
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var product = await _repository.GetByIdAsync(id, ct);
            if (product == null)
            {
                return false;
            }

            _repository.Remove(product);
            await _repository.SaveChangesAsync(ct);
            return true;
        }

        private async Task ValidateReferencesAsync(int? IdCategory, CancellationToken ct)
        {
            if (IdCategory.HasValue && !await _repository.CategoryExistsAsync(IdCategory.Value, ct))
            {
                throw new InvalidOperationException($"Category с Id {IdCategory} не найден.");
            }
        }
    }
}
