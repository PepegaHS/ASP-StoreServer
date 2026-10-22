using AutoMapper;
using StoreServer.models;
using StoreServer.models.DTO;
using StoreServer.Repositories;

namespace StoreServer.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _repository;
        private readonly IMapper _mapper;

        public RoleService(IRoleRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<(List<RoleDTO> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken ct)
        {
            if (page < 1 || pageSize < 1 || (long)(page - 1) * pageSize > int.MaxValue)
            {
                throw new InvalidOperationException("Некорректные параметры пагинации.");
            }

            var items = await _repository.GetAllAsync(page, pageSize, ct);
            var total = await _repository.CountAsync(ct);
            return (_mapper.Map<List<RoleDTO>>(items), total);
        }

        public async Task<RoleDTO?> GetByIdAsync(int id, CancellationToken ct)
        {
            var role = await _repository.GetByIdAsync(id, ct);
            return role == null ? null : _mapper.Map<RoleDTO>(role);
        }

        public async Task<RoleDTO> CreateAsync(CreateRoleDTO dto, CancellationToken ct)
        {
            var role = _mapper.Map<Role>(dto);
            await _repository.AddAsync(role, ct);
            await _repository.SaveChangesAsync(ct);
            role = await _repository.GetByIdAsync(role.IdRole, ct)
                ?? throw new InvalidOperationException("Не удалось загрузить созданную запись.");
            return _mapper.Map<RoleDTO>(role);
        }

        public async Task<RoleDTO?> UpdateAsync(int id, UpdateRoleDTO dto, CancellationToken ct)
        {
            var role = await _repository.GetByIdAsync(id, ct);
            if (role == null)
            {
                return null;
            }

            _mapper.Map(dto, role);
            _repository.Update(role);
            await _repository.SaveChangesAsync(ct);
            return _mapper.Map<RoleDTO>(role);
        }

        public async Task<RoleDTO?> PatchAsync(int id, PatchRoleDTO dto, CancellationToken ct)
        {
            var role = await _repository.GetByIdAsync(id, ct);
            if (role == null)
            {
                return null;
            }

            if (dto.RoleName != null)
            {
                role.RoleName = dto.RoleName;
            }


            _repository.Update(role);
            await _repository.SaveChangesAsync(ct);
            return _mapper.Map<RoleDTO>(role);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var role = await _repository.GetByIdAsync(id, ct);
            if (role == null)
            {
                return false;
            }

            _repository.Remove(role);
            await _repository.SaveChangesAsync(ct);
            return true;
        }
    }
}
