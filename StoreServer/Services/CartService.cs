using AutoMapper;
using StoreServer.models;
using StoreServer.models.DTO;
using StoreServer.Repositories;

namespace StoreServer.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _repository;
        private readonly IMapper _mapper;

        public CartService(ICartRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<(List<CartDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct)
        {
            if (page < 1 || pageSize < 1 || (long)(page - 1) * pageSize > int.MaxValue)
            {
                throw new InvalidOperationException("Некорректные параметры пагинации.");
            }

            var items = await _repository.GetAllAsync(page, pageSize, ct);
            var total = await _repository.CountAsync(ct);
            return (_mapper.Map<List<CartDTO>>(items), total);
        }

        public async Task<CartDTO?> GetByIdAsync(int id, CancellationToken ct)
        {
            var cart = await _repository.GetByIdAsync(id, ct);
            return cart == null ? null : _mapper.Map<CartDTO>(cart);
        }

        public async Task<CartDTO> CreateAsync(CreateCartDTO dto, CancellationToken ct)
        {
            await ValidateReferencesAsync(dto.IdUser, ct);
            var cart = _mapper.Map<Cart>(dto);
            await _repository.AddAsync(cart, ct);
            await _repository.SaveChangesAsync(ct);
            cart = await _repository.GetByIdAsync(cart.IdCart, ct)
                ?? throw new InvalidOperationException("Не удалось загрузить созданную запись.");
            return _mapper.Map<CartDTO>(cart);
        }

        public async Task<CartDTO?> UpdateAsync(int id, UpdateCartDTO dto, CancellationToken ct)
        {
            var cart = await _repository.GetByIdAsync(id, ct);
            if (cart == null)
            {
                return null;
            }

            await ValidateReferencesAsync(dto.IdUser, ct);
            _mapper.Map(dto, cart);
            _repository.Update(cart);
            await _repository.SaveChangesAsync(ct);
            return _mapper.Map<CartDTO>(cart);
        }

        public async Task<CartDTO?> PatchAsync(int id, PatchCartDTO dto, CancellationToken ct)
        {
            var cart = await _repository.GetByIdAsync(id, ct);
            if (cart == null)
            {
                return null;
            }

            await ValidateReferencesAsync(dto.IdUser, ct);
            if (dto.IdUser != null)
            {
                cart.IdUser = dto.IdUser.Value;
            }
            if (dto.CreatedAt != null)
            {
                cart.CreatedAt = dto.CreatedAt.Value;
            }


            _repository.Update(cart);
            await _repository.SaveChangesAsync(ct);
            return _mapper.Map<CartDTO>(cart);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var cart = await _repository.GetByIdAsync(id, ct);
            if (cart == null)
            {
                return false;
            }

            _repository.Remove(cart);
            await _repository.SaveChangesAsync(ct);
            return true;
        }

        private async Task ValidateReferencesAsync(int? IdUser, CancellationToken ct)
        {
            if (IdUser.HasValue && !await _repository.UserExistsAsync(IdUser.Value, ct))
            {
                throw new InvalidOperationException($"User с Id {IdUser} не найден.");
            }
        }
    }
}
