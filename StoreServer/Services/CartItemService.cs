using AutoMapper;
using StoreServer.models;
using StoreServer.models.DTO;
using StoreServer.Repositories;

namespace StoreServer.Services
{
    public class CartItemService : ICartItemService
    {
        private readonly ICartItemRepository _repository;
        private readonly IMapper _mapper;

        public CartItemService(ICartItemRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<(List<CartItemDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct)
        {
            if (page < 1 || pageSize < 1 || (long)(page - 1) * pageSize > int.MaxValue)
            {
                throw new InvalidOperationException("Некорректные параметры пагинации.");
            }

            var items = await _repository.GetAllAsync(page, pageSize, ct);
            var total = await _repository.CountAsync(ct);
            return (_mapper.Map<List<CartItemDTO>>(items), total);
        }

        public async Task<CartItemDTO?> GetByIdAsync(int id, CancellationToken ct)
        {
            var cartItem = await _repository.GetByIdAsync(id, ct);
            return cartItem == null ? null : _mapper.Map<CartItemDTO>(cartItem);
        }

        public async Task<CartItemDTO> CreateAsync(CreateCartItemDTO dto, CancellationToken ct)
        {
            await ValidateReferencesAsync(dto.IdCart, dto.IdProduct, ct);
            var cartItem = _mapper.Map<CartItem>(dto);
            await _repository.AddAsync(cartItem, ct);
            await _repository.SaveChangesAsync(ct);
            cartItem = await _repository.GetByIdAsync(cartItem.IdCartItem, ct)
                ?? throw new InvalidOperationException("Не удалось загрузить созданную запись.");
            return _mapper.Map<CartItemDTO>(cartItem);
        }

        public async Task<CartItemDTO?> UpdateAsync(int id, UpdateCartItemDTO dto, CancellationToken ct)
        {
            var cartItem = await _repository.GetByIdAsync(id, ct);
            if (cartItem == null)
            {
                return null;
            }

            await ValidateReferencesAsync(dto.IdCart, dto.IdProduct, ct);
            _mapper.Map(dto, cartItem);
            _repository.Update(cartItem);
            await _repository.SaveChangesAsync(ct);
            return _mapper.Map<CartItemDTO>(cartItem);
        }

        public async Task<CartItemDTO?> PatchAsync(int id, PatchCartItemDTO dto, CancellationToken ct)
        {
            var cartItem = await _repository.GetByIdAsync(id, ct);
            if (cartItem == null)
            {
                return null;
            }

            await ValidateReferencesAsync(dto.IdCart, dto.IdProduct, ct);
            if (dto.IdCart != null)
            {
                cartItem.IdCart = dto.IdCart.Value;
            }
            if (dto.IdProduct != null)
            {
                cartItem.IdProduct = dto.IdProduct.Value;
            }
            if (dto.Quantity != null)
            {
                cartItem.Quantity = dto.Quantity.Value;
            }


            _repository.Update(cartItem);
            await _repository.SaveChangesAsync(ct);
            return _mapper.Map<CartItemDTO>(cartItem);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var cartItem = await _repository.GetByIdAsync(id, ct);
            if (cartItem == null)
            {
                return false;
            }

            _repository.Remove(cartItem);
            await _repository.SaveChangesAsync(ct);
            return true;
        }

        private async Task ValidateReferencesAsync(int? IdCart, int? IdProduct, CancellationToken ct)
        {
            if (IdCart.HasValue && !await _repository.CartExistsAsync(IdCart.Value, ct))
            {
                throw new InvalidOperationException($"Cart с Id {IdCart} не найден.");
            }
            if (IdProduct.HasValue && !await _repository.ProductExistsAsync(IdProduct.Value, ct))
            {
                throw new InvalidOperationException($"Product с Id {IdProduct} не найден.");
            }
        }
    }
}
