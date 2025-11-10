using BackendPrueba.Models.RolesPermissionsDtos;

namespace BackendPrueba.Services.RolePermissionsService
{
    public interface IRolePermissionService
    {
        Task<IEnumerable<RolePermissionResponseDto>> GetAllAsync();
        Task<RolePermissionResponseDto?> GetByIdAsync(Guid id);
        Task<RolePermissionResponseDto?> CreateAsync(RolePermissionDto request);
        Task<bool> DeleteAsync(Guid id);
    }
}
