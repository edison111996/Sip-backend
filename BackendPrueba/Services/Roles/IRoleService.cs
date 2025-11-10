using BackendPrueba.Models.Roles;

namespace BackendPrueba.Services.Roles
{
   
        public interface IRoleService
        {
            Task<IEnumerable<RolesResponseDto>> GetAllRolesAsync();
            Task<RoleDetailResponseDto?> GetRoleByIdAsync(Guid id);
            Task<RolesResponseDto?> CreateRoleAsync(CreateRoleDto request);
            Task<RolesResponseDto?> UpdateRoleAsync(Guid id, UpdateRoleDto request);
            Task<bool> DeleteRoleAsync(Guid id);
            Task<List<ModulePermissionsDto>> GetAllModulesWithPermissionsAsync(Guid? roleId = null);
        }

    
}
