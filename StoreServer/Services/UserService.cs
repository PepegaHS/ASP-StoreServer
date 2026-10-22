using AutoMapper;
using StoreServer.models;
using StoreServer.models.DTO;
using StoreServer.Repositories;

namespace StoreServer.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<(List<UserDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct)
        {
            if (page < 1 || pageSize < 1 || (long)(page - 1) * pageSize > int.MaxValue)
            {
                throw new InvalidOperationException("Некорректные параметры пагинации.");
            }

            var items = await _repository.GetAllAsync(page, pageSize, ct);
            var total = await _repository.CountAsync(ct);
            return (_mapper.Map<List<UserDTO>>(items), total);
        }

        public async Task<UserDTO?> GetByIdAsync(int id, CancellationToken ct)
        {
            var user = await _repository.GetByIdAsync(id, ct);
            return user == null ? null : _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> CreateAsync(CreateUserDTO dto, CancellationToken ct)
        {
            await ValidateReferencesAsync(dto.IdRole, ct);
            var user = _mapper.Map<User>(dto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            await _repository.AddAsync(user, ct);
            await _repository.SaveChangesAsync(ct);
            user = await _repository.GetByIdAsync(user.IdUser, ct)
                ?? throw new InvalidOperationException("Не удалось загрузить созданную запись.");
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO?> UpdateAsync(int id, UpdateUserDTO dto, CancellationToken ct)
        {
            var user = await _repository.GetByIdAsync(id, ct);
            if (user == null)
            {
                return null;
            }

            await ValidateReferencesAsync(dto.IdRole, ct);
            _mapper.Map(dto, user);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            _repository.Update(user);
            await _repository.SaveChangesAsync(ct);
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO?> PatchAsync(int id, PatchUserDTO dto, CancellationToken ct)
        {
            var user = await _repository.GetByIdAsync(id, ct);
            if (user == null)
            {
                return null;
            }

            await ValidateReferencesAsync(dto.IdRole, ct);
            if (dto.Login != null)
            {
                user.Login = dto.Login;
            }
            if (dto.Password != null)
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }
            if (dto.IdRole.HasValue)
            {
                user.IdRole = dto.IdRole.Value;
            }

            _repository.Update(user);
            await _repository.SaveChangesAsync(ct);
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var user = await _repository.GetByIdAsync(id, ct);
            if (user == null)
            {
                return false;
            }

            _repository.Remove(user);
            await _repository.SaveChangesAsync(ct);
            return true;
        }

        private async Task ValidateReferencesAsync(int? IdRole, CancellationToken ct)
        {
            if (IdRole.HasValue && !await _repository.RoleExistsAsync(IdRole.Value, ct))
            {
                throw new InvalidOperationException($"Role с Id {IdRole} не найден.");
            }
        }
    }
}
