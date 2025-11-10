using BackendPrueba.Data;
using BackendPrueba.Entities;
using BackendPrueba.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BackendPrueba.Services
{
    public class UserService(VideoGameDbContext context) : IUserService
    {
        public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
        {
            return await context.Users
                .Include(u => u.RoleNavigation) // Incluir el rol relacionado
                .Select(u => new UserResponseDto
                {
                    IdUser = u.IdUser,
                    IdRole = u.RoleId,
                    Username = u.Username,
                    Role = u.RoleNavigation.Name
                })
                .ToListAsync();
        }

        public async Task<UserResponseDto?> GetByIdAsync(Guid id)
        {
            var user = await context.Users
                .Include(u => u.RoleNavigation) // 👈 incluir el rol relacionado
                .FirstOrDefaultAsync(u => u.IdUser == id);

            if (user is null) return null;

            return new UserResponseDto
            {
                IdUser = user.IdUser,
                IdRole = user.RoleId,              // 👈 ID del rol (Guid)
                Username = user.Username,
                Role = user.RoleNavigation.Name    // 👈 nombre del rol (string)
            };
        }

        public async Task<UserResponseDto?> CreateAsync(UserDto request)
        {
            // Verificar si el usuario ya existe
            if (await context.Users.AnyAsync(u => u.Username == request.Username))
                return null;

            // Verificar que el rol exista
            var role = await context.Roles.FindAsync(request.RoleId);
            if (role == null)
                throw new Exception("El rol especificado no existe.");

            // Crear el usuario
            var user = new User
            {
                IdUser = Guid.NewGuid(),
                Username = request.Username,
                PasswordHash = new PasswordHasher<User>().HashPassword(new User(), request.Password),
                RoleId = role.IdRole, // FK
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return new UserResponseDto
            {
                IdUser = user.IdUser,
                IdRole = user.RoleId,
                Username = user.Username,
                Role = role.Name
            };
        }

        public async Task<UserResponseDto?> UpdateAsync(Guid id, UpdateUserDto request)
        {
            var user = await context.Users.Include(u => u.RoleNavigation) 
                            .FirstOrDefaultAsync(u => u.IdUser == id);
            if (user is null) return null;

            user.Username = request.Username;
            user.RoleId = request.RoleId;

            if (!string.IsNullOrEmpty(request.Password))
            {
                user.PasswordHash = new PasswordHasher<User>().HashPassword(user, request.Password);
            }
            user.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return new UserResponseDto
            {
                IdUser = user.IdUser,
                Username = user.Username,
                IdRole = user.RoleId,
                Role = user.RoleNavigation?.Name ?? ""
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await context.Users.FindAsync(id);
            if (user is null) return false;

            context.Users.Remove(user);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
