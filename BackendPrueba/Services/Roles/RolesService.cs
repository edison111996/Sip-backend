using AutoMapper;
using BackendPrueba.Data;
using BackendPrueba.Entities;
using BackendPrueba.Models.PermissionDtos;
using BackendPrueba.Models.Roles;
using Microsoft.EntityFrameworkCore;

namespace BackendPrueba.Services.Roles
{
    public class RolesService(VideoGameDbContext context, IMapper mapper) : IRoleService
    {
        // Obtener todos los roles
        public async Task<IEnumerable<RolesResponseDto>> GetAllRolesAsync()
        {
            var roles = await context.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                        .ThenInclude(p => p.Module) // Si tienes la relación
                .OrderBy(r => r.Name)
                .ToListAsync();

            var result = roles.Select(role => new RolesResponseDto
            {
                IdRole = role.IdRole,
                Name = role.Name,
                Description = role.Description,
                IsSystem = role.IsSystem,
                CreatedAt = role.CreatedAt,
                UpdatedAt = role.UpdatedAt,
                Permissions = role.RolePermissions.Select(rp => new PermissionWithModuleDto
                {
                    IdPermission = rp.PermissionId,
                    Name = rp.Permission.Name,
                    Description = rp.Permission.Description,
                    ModuleId = rp.Permission.ModuleId,
                    ModuleName = rp.Permission.Module != null ? rp.Permission.Module.Name : null
                }).ToList()
            });

            return result;
        }


        // Obtener rol por ID con sus permisos agrupados por módulo
        public async Task<RoleDetailResponseDto?> GetRoleByIdAsync(Guid id)
        {
            var role = await context.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                        .ThenInclude(p => p.Module)
                .FirstOrDefaultAsync(r => r.IdRole == id);

            if (role is null) return null;

            var roleDto = mapper.Map<RoleDetailResponseDto>(role);

            // Agrupar permisos por módulo
            roleDto.Modules = role.RolePermissions
                .GroupBy(rp => rp.Permission.Module)
                .Select(g => new ModulePermissionsDto
                {
                    IdModule = g.Key.IdModule,
                    ModuleName = g.Key.Name,
                    ModuleDescription = g.Key.Description,
                    Permissions = g.Select(rp => new PermissionDto
                    {
                        IdPermission = rp.Permission.IdPermission,
                        Name = rp.Permission.Name,
                        Description = rp.Permission.Description,
                        IsAssigned = true
                    }).ToList()
                })
                .OrderBy(m => m.ModuleName)
                .ToList();

            return roleDto;
        }

        // Crear nuevo rol con permisos
        public async Task<RolesResponseDto?> CreateRoleAsync(CreateRoleDto request)
        {
            // Verificar si el nombre ya existe
            if (await context.Roles.AnyAsync(r => r.Name == request.Name))
                return null;

            // Validar que los permisos existan
            var existingPermissions = await context.Permissions
                .Where(p => request.PermissionIds.Contains(p.IdPermission))
                .Select(p => p.IdPermission)
                .ToListAsync();

            if (existingPermissions.Count != request.PermissionIds.Count)
                return null; // Algunos permisos no existen

            var role = mapper.Map<Role>(request);
            role.IdRole = Guid.NewGuid();
            role.CreatedAt = DateTime.UtcNow;
            role.IsSystem = false;

            // Asignar permisos
            role.RolePermissions = request.PermissionIds.Select(permId => new RolePermission
            {
                IdRolePermission = Guid.NewGuid(),
                RoleId = role.IdRole,
                PermissionId = permId
            }).ToList();

            context.Roles.Add(role);
            await context.SaveChangesAsync();

            return mapper.Map<RolesResponseDto>(role);
        }

        // Actualizar rol y sus permisos
        public async Task<RolesResponseDto?> UpdateRoleAsync(Guid id, UpdateRoleDto request)
        {
            var role = await context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.IdRole == id);

            if (role is null) return null;
            if (role.IsSystem) return null;

            // Verificar nombre único
            if (await context.Roles.AnyAsync(r => r.Name == request.Name && r.IdRole != id))
                return null;

            // Validar permisos
            var existingPermissions = await context.Permissions
                .Where(p => request.PermissionIds.Contains(p.IdPermission))
                .Select(p => p.IdPermission)
                .ToListAsync();

            if (existingPermissions.Count != request.PermissionIds.Count)
                return null;

            // Actualizar datos del rol
            role.Name = request.Name;
            role.Description = request.Description;
            role.UpdatedAt = DateTime.UtcNow;

            // Eliminar todos los permisos actuales
            context.RolePermissions.RemoveRange(role.RolePermissions);

            // Agregar todos los nuevos permisos seleccionados (puede ser vacío)
            var newRolePermissions = request.PermissionIds.Select(pid => new RolePermission
            {
                IdRolePermission = Guid.NewGuid(),
                RoleId = role.IdRole,
                PermissionId = pid
            }).ToList();

            await context.RolePermissions.AddRangeAsync(newRolePermissions);

            await context.SaveChangesAsync();

            return mapper.Map<RolesResponseDto>(role);
        }
        // Eliminar rol
        public async Task<bool> DeleteRoleAsync(Guid id)
        {
            var role = await context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.IdRole == id);

            if (role is null || role.IsSystem) return false;

            context.RolePermissions.RemoveRange(role.RolePermissions);
            context.Roles.Remove(role);
            await context.SaveChangesAsync();

            return true;
        }


        // Obtener todos los módulos con permisos (para el formulario)
        public async Task<List<ModulePermissionsDto>> GetAllModulesWithPermissionsAsync(Guid? roleId = null)
        {
            var modules = await context.Module
                .Include(m => m.Permissions)
                .OrderBy(m => m.Name)
                .ToListAsync();

            // Si se proporciona un roleId, marcar los permisos asignados
            List<Guid> assignedPermissionIds = new();
            if (roleId.HasValue)
            {
                assignedPermissionIds = await context.RolePermissions
                    .Where(rp => rp.RoleId == roleId.Value)
                    .Select(rp => rp.PermissionId)
                    .ToListAsync();
            }

            return modules.Select(m => new ModulePermissionsDto
            {
                IdModule = m.IdModule,
                ModuleName = m.Name,
                ModuleDescription = m.Description,
                Permissions = m.Permissions
                    .OrderBy(p => p.Name)
                    .Select(p => new PermissionDto
                    {
                        IdPermission = p.IdPermission,
                        Name = p.Name,
                        Description = p.Description,
                        IsAssigned = assignedPermissionIds.Contains(p.IdPermission)
                    }).ToList()
            }).ToList();
        }
    }
}

