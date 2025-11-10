using BackendPrueba.Entities;
using BackendPrueba.Models;

namespace BackendPrueba.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDto request);
        Task<TokenResponseDto?> LoginAsync(UserDto request);

        Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto refresh);

        Task<bool> RevokeRefreshTokenAsync(string refreshToken);

        // Nuevo método para buscar usuario por refresh token
        Task<User?> GetUserByRefreshTokenAsync(string refreshToken);

        // Nuevo método para crear el token response
        Task<TokenResponseDto> CreateTokenResponse(User user);


    }
}
