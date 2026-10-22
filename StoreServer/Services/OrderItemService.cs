using AutoMapper;
using StoreServer.models;
using StoreServer.models.DTO;
using StoreServer.Repositories;

namespace StoreServer.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IOrderItemRepository _repository;
        private readonly IMapper _mapper;

        public OrderItemService(IOrderItemRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<(List<OrderItemDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct)
        {
            if (page < 1 || pageSize < 1 || (long)(page - 1) * pageSize > int.MaxValue)
            {
                throw new InvalidOperationException("Некорректные параметры пагинации.");
            }

            var items = await _repository.GetAllAsync(page, pageSize, ct);
            var total = await _repository.CountAsync(ct);
            return (_mapper.Map<List<OrderItemDTO>>(items), total);
        }

        public async Task<OrderItemDTO?> GetByIdAsync(int id, CancellationToken ct)
        {
            var orderItem = await _repository.GetByIdAsync(id, ct);
            return orderItem == null ? null : _mapper.Map<OrderItemDTO>(orderItem);
        }

        public async Task<OrderItemDTO> CreateAsync(CreateOrderItemDTO dto, CancellationToken ct)
        {
            await ValidateReferencesAsync(dto.IdOrder, dto.IdProduct, ct);
            var orderItem = _mapper.Map<OrderItem>(dto);
            await _repository.AddAsync(orderItem, ct);
            await _repository.SaveChangesAsync(ct);
            orderItem = await _repository.GetByIdAsync(orderItem.IdOrderItem, ct)
                ?? throw new InvalidOperationException("Не удалось загрузить созданную запись.");
            return _mapper.Map<OrderItemDTO>(orderItem);
        }

        public async Task<OrderItemDTO?> UpdateAsync(int id, UpdateOrderItemDTO dto, CancellationToken ct)
        {
            var orderItem = await _repository.GetByIdAsync(id, ct);
            if (orderItem == null)
            {
                return null;
            }

            await ValidateReferencesAsync(dto.IdOrder, dto.IdProduct, ct);
            _mapper.Map(dto, orderItem);
            _repository.Update(orderItem);
            await _repository.SaveChangesAsync(ct);
            return _mapper.Map<OrderItemDTO>(orderItem);
        }

        public async Task<OrderItemDTO?> PatchAsync(int id, PatchOrderItemDTO dto, CancellationToken ct)
        {
            var orderItem = await _repository.GetByIdAsync(id, ct);
            if (orderItem == null)
            {
                return null;
            }

            await ValidateReferencesAsync(dto.IdOrder, dto.IdProduct, ct);
            if (dto.IdOrder != null)
            {
                orderItem.IdOrder = dto.IdOrder.Value;
            }
            if (dto.IdProduct != null)
            {
                orderItem.IdProduct = dto.IdProduct.Value;
            }
            if (dto.Quantity != null)
            {
                orderItem.Quantity = dto.Quantity.Value;
            }
            if (dto.PriceAtMoment != null)
            {
                orderItem.PriceAtMoment = dto.PriceAtMoment.Value;
            }


            _repository.Update(orderItem);
            await _repository.SaveChangesAsync(ct);
            return _mapper.Map<OrderItemDTO>(orderItem);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var orderItem = await _repository.GetByIdAsync(id, ct);
            if (orderItem == null)
            {
                return false;
            }

            _repository.Remove(orderItem);
            await _repository.SaveChangesAsync(ct);
            return true;
        }

        private async Task ValidateReferencesAsync(int? IdOrder, int? IdProduct, CancellationToken ct)
        {
            if (IdOrder.HasValue && !await _repository.OrderExistsAsync(IdOrder.Value, ct))
            {
                throw new InvalidOperationException($"Order с Id {IdOrder} не найден.");
            }
            if (IdProduct.HasValue && !await _repository.ProductExistsAsync(IdProduct.Value, ct))
            {
                throw new InvalidOperationException($"Product с Id {IdProduct} не найден.");
            }
        }
    }
}
