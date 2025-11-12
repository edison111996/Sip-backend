using BackendPrueba.Data;
using BackendPrueba.Entities;
using BackendPrueba.Middleware;
using BackendPrueba.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace BackendPrueba.Services
{
    public class AuthService(VideoGameDbContext context, IConfiguration configuration, SymmetricSecurityKey signingKey) : IAuthService
    {

        //public async Task<TokenResponseDto?> LoginAsync(UserDto request)
        //{
        //    var user = context.Users.FirstOrDefault(u => u.Username == request.Username);
        //    if (user is null)
        //    {
        //        return null;
        //    }
        //    if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password)
        //        == PasswordVerificationResult.Failed)
        //    {
        //        return null;
        //    }

        //    var response = new TokenResponseDto
        //    {
        //        AccessToken = CreateToken(user),
        //        RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
        //    };


        //    return await CreateTokenResponse(user);
        //}

        public async Task<TokenResponseDto> LoginAsync(UserDto request)
        {
            // 1. Buscar el usuario
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            // 2. Verificar si el usuario existe
            if (user is null)
            {
                throw new UserNotFoundException(request.Username);  // ✅ Excepción específica
            }

            // 3. Verificar la contraseña
            var verificationResult = new PasswordHasher<User>()
                .VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                throw new InvalidPasswordException();  // ✅ Excepción específica
            }

            // 4. Si todo está bien, crear y devolver el token
            return await CreateTokenResponse(user);
        }

        public async Task<TokenResponseDto> CreateTokenResponse(User user)
        {
            return new TokenResponseDto
            {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
            };
        }

        public async Task<User?> RegisterAsync(UserDto request)
        {
            if (await context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return null;
            }
            var user = new User();
            var hashedPassword = new PasswordHasher<User>()
            .HashPassword(user, request.Password);

            user.Username = request.Username;
            user.PasswordHash = hashedPassword;

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return user;
        }

        public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
            if (user is null)
                return null;

            // ✅ ROTACIÓN: Invalida el token anterior
            user.RefreshToken = null;
            await context.SaveChangesAsync();

            // Genera nuevos tokens
            return await CreateTokenResponse(user);
        }

        private async Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var user = await context.Users
                .Include(u => u.RoleId) // Si necesitas el rol
                .FirstOrDefaultAsync(u => u.IdUser == userId);

            if (user is null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(user.RefreshToken))
            {
                return null;
            }

            if (user.RefreshToken != refreshToken)
            {
                return null;
            }

            if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }

            return user;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.RefreshTokenExpiryTime > DateTime.UtcNow);
        }

        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            var refreshTokenDays = configuration.GetValue<int>("AppSettings:RefreshTokenExpirationDays", 7);
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshTokenDays);

            // ⚠️ AGREGÁ ESTA LÍNEA - SIN ESTO EL TOKEN NO SE GUARDA
            await context.SaveChangesAsync();

            return refreshToken;
        }


        // ============================================
        // CREATE ACCESS TOKEN (JWT)
        // ============================================
        private string CreateToken(User user)
        {
            // ✅ Claims mejorados
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.IdUser.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                new("idUser", user.IdUser.ToString()),
                new("username", user.Username),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.NameIdentifier, user.IdUser.ToString())
            };

            // ✅ Agregar rol si existe
            if (user.RoleId != Guid.Empty)
            {
                claims.Add(new Claim(ClaimTypes.Role, user.RoleId.ToString()));
                claims.Add(new Claim("roleId", user.RoleId.ToString()));
            }

            // ✅ Usar clave inyectada (cacheada)
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            // ✅ Tiempo de expiración desde configuración
            var tokenExpirationMinutes = configuration.GetValue<int>("AppSettings:TokenExpirationMinutes", 60);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration["AppSettings:Issuer"],
                audience: configuration["AppSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(tokenExpirationMinutes),
                signingCredentials: creds
            );

            var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);


            return token;
        }
        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
        {
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user is null || user.RefreshToken != refreshToken)
                return false;

            // Invalida el token
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.UtcNow;
            await context.SaveChangesAsync();

            return true;
        }
    }
}
