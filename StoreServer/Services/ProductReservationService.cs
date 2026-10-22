using AutoMapper;
using StoreServer.models;
using StoreServer.models.DTO;
using StoreServer.Repositories;

namespace StoreServer.Services
{
    public class ProductReservationService : IProductReservationService
    {
        private readonly IProductReservationRepository _repository;
        private readonly IMapper _mapper;

        public ProductReservationService(IProductReservationRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<(List<ProductReservationDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct)
        {
            if (page < 1 || pageSize < 1 || (long)(page - 1) * pageSize > int.MaxValue)
            {
                throw new InvalidOperationException("Некорректные параметры пагинации.");
            }

            var items = await _repository.GetAllAsync(page, pageSize, ct);
            var total = await _repository.CountAsync(ct);
            return (_mapper.Map<List<ProductReservationDTO>>(items), total);
        }

        public async Task<ProductReservationDTO?> GetByIdAsync(int id, CancellationToken ct)
        {
            var reservation = await _repository.GetByIdAsync(id, ct);
            return reservation == null ? null : _mapper.Map<ProductReservationDTO>(reservation);
        }

        public async Task<ProductReservationDTO> CreateAsync(CreateProductReservationDTO dto, CancellationToken ct)
        {
            if (dto.Quantity <= 0)
            {
                throw new InvalidOperationException("Количество должно быть больше нуля.");
            }

            var product = await _repository.FindProductAsync(dto.IdProduct, ct)
                ?? throw new InvalidOperationException("Товар не найден.");
            if (!await _repository.UserExistsAsync(dto.IdUser, ct))
            {
                throw new InvalidOperationException("Пользователь не найден.");
            }
            if (product.AvailableQuantity < dto.Quantity)
            {
                throw new InvalidOperationException($"Недостаточно товара. Доступно: {product.AvailableQuantity}.");
            }

            var reservation = _mapper.Map<ProductReservation>(dto);
            reservation.CreatedAt = DateTime.UtcNow;
            product.AvailableQuantity -= dto.Quantity;
            await _repository.AddAsync(reservation, ct);
            await _repository.SaveChangesAsync(ct);

            reservation = await _repository.GetByIdAsync(reservation.IdProductReservation, ct)
                ?? throw new InvalidOperationException("Не удалось загрузить созданный резерв.");
            return _mapper.Map<ProductReservationDTO>(reservation);
        }

        public async Task<ProductReservationDTO?> UpdateAsync(int id, UpdateProductReservationDTO dto, CancellationToken ct)
        {
            var reservation = await _repository.GetByIdAsync(id, ct);
            if (reservation == null || reservation.CancelledAt != null)
            {
                return null;
            }

            AdjustQuantity(reservation, dto.Quantity);
            _mapper.Map(dto, reservation);
            await _repository.SaveChangesAsync(ct);
            return _mapper.Map<ProductReservationDTO>(reservation);
        }

        public async Task<ProductReservationDTO?> PatchAsync(int id, PatchProductReservationDTO dto, CancellationToken ct)
        {
            var reservation = await _repository.GetByIdAsync(id, ct);
            if (reservation == null || reservation.CancelledAt != null)
            {
                return null;
            }

            if (dto.Quantity.HasValue)
            {
                AdjustQuantity(reservation, dto.Quantity.Value);
                _mapper.Map(new UpdateProductReservationDTO { Quantity = dto.Quantity.Value }, reservation);
            }
            await _repository.SaveChangesAsync(ct);
            return _mapper.Map<ProductReservationDTO>(reservation);
        }

        public async Task<bool> CancelAsync(int id, CancellationToken ct)
        {
            var reservation = await _repository.GetByIdAsync(id, ct);
            if (reservation == null || reservation.CancelledAt != null)
            {
                return false;
            }
            if ((long)reservation.Product.AvailableQuantity + reservation.Quantity > int.MaxValue)
            {
                throw new InvalidOperationException("Возврат превысит допустимый остаток товара.");
            }

            reservation.Product.AvailableQuantity += reservation.Quantity;
            reservation.CancelledAt = DateTime.UtcNow;
            await _repository.SaveChangesAsync(ct);
            return true;
        }

        private static void AdjustQuantity(ProductReservation reservation, int quantity)
        {
            if (quantity <= 0)
            {
                throw new InvalidOperationException("Количество должно быть больше нуля.");
            }

            var available = (long)reservation.Product.AvailableQuantity + reservation.Quantity - quantity;
            if (available < 0 || available > int.MaxValue)
            {
                throw new InvalidOperationException("Недостаточно товара или превышен допустимый остаток.");
            }
            reservation.Product.AvailableQuantity = (int)available;
        }
    }
}
