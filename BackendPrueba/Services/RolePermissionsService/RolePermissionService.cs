using BackendPrueba.Data;
using BackendPrueba.Entities;
using BackendPrueba.Models.RolesPermissionsDtos;
using Microsoft.EntityFrameworkCore;

namespace BackendPrueba.Services.RolePermissionsService
{
    public class RolePermissionService(VideoGameDbContext context) : IRolePermissionService
    {
        public async Task<IEnumerable<RolePermissionResponseDto>> GetAllAsync()
        {
            return await context.RolePermissions
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .Select(rp => new RolePermissionResponseDto
                {
                    IdRolePermission = rp.RoleId,
                    RoleId = rp.RoleId,
                    RoleName = rp.Role.Name,
                    PermissionId = rp.PermissionId,
                    PermissionName = rp.Permission.Name
                })
                .ToListAsync();
        }

        public async Task<RolePermissionResponseDto?> GetByIdAsync(Guid id)
        {
            var rp = await context.RolePermissions
                .Include(r => r.Role)
                .Include(p => p.Permission)
                .FirstOrDefaultAsync(x => x.RoleId == id);

            if (rp == null) return null;

            return new RolePermissionResponseDto
            {
                IdRolePermission = rp.RoleId,
                RoleId = rp.RoleId,
                RoleName = rp.Role.Name,
                PermissionId = rp.PermissionId,
                PermissionName = rp.Permission.Name
            };
        }

        public async Task<RolePermissionResponseDto?> CreateAsync(RolePermissionDto request)
        {
            // Evita duplicados
            bool exists = await context.RolePermissions
                .AnyAsync(rp => rp.RoleId == request.RoleId && rp.PermissionId == request.PermissionId);
            if (exists) return null;

            var entity = new RolePermission
            {
                RoleId = request.RoleId,
                PermissionId = request.PermissionId
            };

            context.RolePermissions.Add(entity);
            await context.SaveChangesAsync();

            var role = await context.Roles.FindAsync(request.RoleId);
            var permission = await context.Permissions.FindAsync(request.PermissionId);

            return new RolePermissionResponseDto
            {
                IdRolePermission = entity.RoleId,
                RoleId = entity.RoleId,
                RoleName = role?.Name ?? "",
                PermissionId = entity.PermissionId,
                PermissionName = permission?.Name ?? ""
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var rp = await context.RolePermissions.FindAsync(id);
            if (rp == null) return false;

            context.RolePermissions.Remove(rp);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
