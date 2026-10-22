using AutoMapper;
using StoreServer.models.DTO;
using StoreServer.Repositories;

namespace StoreServer.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public AuthService(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<LoginResponseDTO?> LoginAsync(LoginDTO dto, CancellationToken ct)
        {
            var user = await _repository.FindByLoginAsync(dto.Login, ct);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return null;
            }

            return _mapper.Map<LoginResponseDTO>(user);
        }
    }
}
