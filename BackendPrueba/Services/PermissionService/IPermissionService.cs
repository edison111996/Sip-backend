using BackendPrueba.Models.PermissionDtos;

namespace BackendPrueba.Services.PermissionService
{
    public interface IPermissionService
    {
        Task<IEnumerable<PermissionResponseDto>> GetAllPermissionsAsync();
        Task<PermissionResponseDto?> GetPermissionByIdAsync(Guid id);
        Task<PermissionResponseDto?> CreatePermissionAsync(PermissionDto request);
        Task<PermissionResponseDto?> UpdatePermissionAsync(Guid id, PermissionDto request);
        Task<bool> DeletePermissionAsync(Guid id);
    }
}
