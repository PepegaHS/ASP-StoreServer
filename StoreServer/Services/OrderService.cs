using AutoMapper;
using StoreServer.models;
using StoreServer.models.DTO;
using StoreServer.Repositories;

namespace StoreServer.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<(List<OrderDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct)
        {
            if (page < 1 || pageSize < 1 || (long)(page - 1) * pageSize > int.MaxValue)
            {
                throw new InvalidOperationException("Некорректные параметры пагинации.");
            }

            var items = await _repository.GetAllAsync(page, pageSize, ct);
            var total = await _repository.CountAsync(ct);
            return (_mapper.Map<List<OrderDTO>>(items), total);
        }

        public async Task<OrderDTO?> GetByIdAsync(int id, CancellationToken ct)
        {
            var order = await _repository.GetByIdAsync(id, ct);
            return order == null ? null : _mapper.Map<OrderDTO>(order);
        }

        public async Task<OrderDTO> CreateAsync(CreateOrderDTO dto, CancellationToken ct)
        {
            await ValidateReferencesAsync(dto.IdUser, ct);
            var order = _mapper.Map<Order>(dto);
            await _repository.AddAsync(order, ct);
            await _repository.SaveChangesAsync(ct);
            order = await _repository.GetByIdAsync(order.IdOrder, ct)
                ?? throw new InvalidOperationException("Не удалось загрузить созданную запись.");
            return _mapper.Map<OrderDTO>(order);
        }

        public async Task<OrderDTO?> UpdateAsync(int id, UpdateOrderDTO dto, CancellationToken ct)
        {
            var order = await _repository.GetByIdAsync(id, ct);
            if (order == null)
            {
                return null;
            }

            await ValidateReferencesAsync(dto.IdUser, ct);
            _mapper.Map(dto, order);
            _repository.Update(order);
            await _repository.SaveChangesAsync(ct);
            return _mapper.Map<OrderDTO>(order);
        }

        public async Task<OrderDTO?> PatchAsync(int id, PatchOrderDTO dto, CancellationToken ct)
        {
            var order = await _repository.GetByIdAsync(id, ct);
            if (order == null)
            {
                return null;
            }

            await ValidateReferencesAsync(dto.IdUser, ct);
            if (dto.IdUser != null)
            {
                order.IdUser = dto.IdUser.Value;
            }
            if (dto.OrderDate != null)
            {
                order.OrderDate = dto.OrderDate.Value;
            }
            if (dto.TotalAmount != null)
            {
                order.TotalAmount = dto.TotalAmount.Value;
            }
            if (dto.OrderStatus != null)
            {
                order.OrderStatus = dto.OrderStatus;
            }


            _repository.Update(order);
            await _repository.SaveChangesAsync(ct);
            return _mapper.Map<OrderDTO>(order);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var order = await _repository.GetByIdAsync(id, ct);
            if (order == null)
            {
                return false;
            }

            _repository.Remove(order);
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
