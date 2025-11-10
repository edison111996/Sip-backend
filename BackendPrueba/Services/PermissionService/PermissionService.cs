using BackendPrueba.Data;
using BackendPrueba.Entities;
using BackendPrueba.Models.PermissionDtos;
using Microsoft.EntityFrameworkCore;

namespace BackendPrueba.Services.PermissionService
{
    public class PermissionService(VideoGameDbContext context) : IPermissionService
    {
        public async Task<IEnumerable<PermissionResponseDto>> GetAllPermissionsAsync()
        {
            return await context.Permissions
                .Select(p => new PermissionResponseDto
                {
                    IdPermission = p.IdPermission,
                    Name = p.Name,
                    Description = p.Description,
                    ModuleId = p.ModuleId,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<PermissionResponseDto?> GetPermissionByIdAsync(Guid id)
        {
            var permission = await context.Permissions.FindAsync(id);
            if (permission is null) return null;

            return new PermissionResponseDto
            {
                IdPermission = permission.IdPermission,
                Name = permission.Name,
                Description = permission.Description,
                ModuleId = permission.ModuleId,
                CreatedAt = permission.CreatedAt
            };
        }

        public async Task<PermissionResponseDto?> CreatePermissionAsync(PermissionDto request)
        {
            if (await context.Permissions.AnyAsync(p => p.Name == request.Name))
                return null;

            var permission = new Permission
            {
                IdPermission = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                ModuleId = request.ModuleId,
                CreatedAt = DateTime.UtcNow
            };

            context.Permissions.Add(permission);
            await context.SaveChangesAsync();

            return new PermissionResponseDto
            {
                IdPermission = permission.IdPermission,
                Name = permission.Name,
                Description = permission.Description,
                ModuleId = permission.ModuleId,
                CreatedAt = permission.CreatedAt
            };
        }

        public async Task<PermissionResponseDto?> UpdatePermissionAsync(Guid id, PermissionDto request)
        {
            var permission = await context.Permissions.FindAsync(id);
            if (permission is null) return null;

            permission.Name = request.Name;
            permission.Description = request.Description;
            permission.ModuleId = request.ModuleId;

            await context.SaveChangesAsync();

            return new PermissionResponseDto
            {
                IdPermission = permission.IdPermission,
                Name = permission.Name,
                Description = permission.Description,
                ModuleId = permission.ModuleId,
                CreatedAt = permission.CreatedAt
            };
        }

        public async Task<bool> DeletePermissionAsync(Guid id)
        {
            var permission = await context.Permissions.FindAsync(id);
            if (permission is null) return false;

            context.Permissions.Remove(permission);
            await context.SaveChangesAsync();
            return true;
        }
    }
}

