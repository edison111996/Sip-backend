using BackendPrueba.Models;

namespace BackendPrueba.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAllAsync();
        Task<UserResponseDto?> GetByIdAsync(Guid id);
        Task<UserResponseDto?> CreateAsync(UserDto request);
        Task<UserResponseDto?> UpdateAsync(Guid id, UpdateUserDto request);
        Task<bool> DeleteAsync(Guid id);
    }
}
