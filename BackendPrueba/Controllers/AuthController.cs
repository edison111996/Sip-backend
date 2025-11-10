//using BackendPrueba.Entities;
//using BackendPrueba.Models;
//using BackendPrueba.Services;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace BackendPrueba.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AuthController(IAuthService authService) : ControllerBase
//    {
//        public static User user = new();


//        [HttpPost("register")]
//        public async Task<ActionResult<User>> Register(UserDto request)
//        {
//            var user = await authService.RegisterAsync(request);
//            if (user is null)
//                return BadRequest("User already exists.");

//            return Ok(user);
//        }

//        //[HttpPost("login")]
//        //public async Task<ActionResult<TokenResponseDto>> Login(UserDto request)
//        //{
//        //    var result = await authService.LoginAsync(request);
//        //    if (result is null)
//        //        return BadRequest("Invalid username or password.");

//        //    // Envía el refresh token como cookie httpOnly
//        //    Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
//        //    {
//        //        HttpOnly = true,
//        //        Secure = true, 
//        //        SameSite = SameSiteMode.None,
//        //        Expires = DateTime.UtcNow.AddDays(7),
//        //        Path = "/"
//        //    });

//        //    // Solo envía el access token en el body
//        //    return Ok(new { accessToken = result.AccessToken });
//        //}
//        [HttpPost("login")]
//        public async Task<ActionResult<TokenResponseDto>> Login(UserDto request)
//        {
//            var result = await authService.LoginAsync(request);
//            if (result is null)
//                return BadRequest("Invalid username or password.");

//            // Envía el refresh token como cookie httpOnly
//            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
//            {
//                HttpOnly = true,
//                Secure = true,
//                SameSite = SameSiteMode.None,
//                Expires = DateTime.UtcNow.AddDays(7),
//                Path = "/"
//            });

//            // Solo envía el access token en el body
//            return Ok(new { accessToken = result.AccessToken });
//        }




//        //[HttpPost("refresh-token")]
//        //public async Task<ActionResult<TokenResponseDto>> RefreshToken()
//        //{
//        //    var refreshToken = Request.Cookies["refreshToken"];
//        //    if (string.IsNullOrEmpty(refreshToken))
//        //        return Unauthorized("No refresh token found.");

//        //    // Busca el usuario por el refresh token
//        //    var user = await authService.GetUserByRefreshTokenAsync(refreshToken);
//        //    if (user is null)
//        //        return Unauthorized("Invalid refresh token.");

//        //    var result = await authService.CreateTokenResponse(user);
//        //    if (result is null || result.AccessToken is null || result.RefreshToken is null)
//        //        return Unauthorized("Could not generate new access token.");

//        //    // Actualiza la cookie con el nuevo refresh token
//        //    Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
//        //    {
//        //        HttpOnly = true,
//        //        Secure = true,
//        //        SameSite = SameSiteMode.Strict,
//        //        Expires = DateTime.UtcNow.AddDays(7),
//        //        Path = "/"
//        //    });

//        //    return Ok(new { accessToken = result.AccessToken });
//        //}

//        [HttpPost("refresh-token")]
//        public async Task<ActionResult<TokenResponseDto>> RefreshToken()
//        {
//            var refreshToken = Request.Cookies["refreshToken"];
//            if (string.IsNullOrEmpty(refreshToken))
//                return Unauthorized("No refresh token found.");

//            var user = await authService.GetUserByRefreshTokenAsync(refreshToken);
//            if (user is null)
//                return Unauthorized("Invalid refresh token.");

//            var result = await authService.CreateTokenResponse(user);
//            if (result is null || result.AccessToken is null || result.RefreshToken is null)
//                return Unauthorized("Could not generate new access token.");

//            // Actualiza la cookie con el nuevo refresh token
//            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
//            {
//                HttpOnly = true,
//                Secure = true,
//                SameSite = SameSiteMode.None, // ✅ Cambiado a None para coincidir con login
//                Expires = DateTime.UtcNow.AddDays(7),
//                Path = "/"
//            });

//            return Ok(new { accessToken = result.AccessToken });
//        }

//        [Authorize]
//        [HttpGet]
//        public IActionResult AuthenticatedOnlyEndpoint()
//        {
//            return Ok("You are authenticated");
//        }


//        [Authorize(Roles = "Admin")]
//        [HttpGet("admin-only")]
//        public IActionResult AdminOnlyEndpoint()
//        {
//            return Ok("You are authenticated");
//        }


//        //[HttpPost("logout")]
//        //public async Task<IActionResult> Logout([FromBody] LogoutRequestDto request)
//        //{
//        //    var result = await authService.RevokeRefreshTokenAsync(request.RefreshToken);
//        //    if (!result)
//        //        return BadRequest("Invalid or already revoked token.");

//        //    // Sobrescribe la cookie con expiración pasada
//        //    Response.Cookies.Append("refreshToken", "", new CookieOptions
//        //    {
//        //        Expires = DateTimeOffset.UtcNow.AddDays(-1),
//        //        HttpOnly = true,
//        //        Secure = true,
//        //        SameSite = SameSiteMode.Strict,
//        //        Path = "/"
//        //    });

//        //    return Ok("Logout successful.");
//        //}

//        [HttpPost("logout")]
//        public async Task<IActionResult> Logout()
//        {
//            // Lee el refresh token de la cookie
//            var refreshToken = Request.Cookies["refreshToken"];

//            if (!string.IsNullOrEmpty(refreshToken))
//            {
//                // Revoca el token en la base de datos (si guardas tokens)
//                await authService.RevokeRefreshTokenAsync(refreshToken);
//            }

//            // CRÍTICO: Elimina la cookie con las MISMAS opciones que al crearla
//            Response.Cookies.Delete("refreshToken", new CookieOptions
//            {
//                HttpOnly = true,
//                Secure = true,
//                SameSite = SameSiteMode.None, // DEBE ser igual que en login
//                Path = "/"
//            });

//            return Ok(new { message = "Logout successful" });
//        }
//    }
//}

using BackendPrueba.Entities;
using BackendPrueba.Models;
using BackendPrueba.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendPrueba.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly ILogger<AuthController> _logger = logger;

        // ============================================
        // CONFIGURACIÓN DE COOKIES CENTRALIZADA
        // ============================================
        private CookieOptions GetRefreshTokenCookieOptions()
        {
            return new CookieOptions
            {
                HttpOnly = true,              // 🔒 No accesible por JavaScript
                Secure = true,                // 🔒 Solo HTTPS
                SameSite = SameSiteMode.None, // 🔒 Permite cross-origin (necesario para localhost)
                Expires = DateTime.UtcNow.AddDays(7),
                Path = "/"                    // 🔒 Disponible en toda la app
            };
        }

        // ============================================
        // REGISTER
        // ============================================
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            try
            {
                _logger.LogInformation("Intento de registro para usuario: {Username}", request.Username);

                var user = await _authService.RegisterAsync(request);
                if (user is null)
                {
                    _logger.LogWarning("Usuario ya existe: {Username}", request.Username);
                    return BadRequest(new { message = "El usuario ya existe." });
                }

                _logger.LogInformation("Usuario registrado exitosamente: {Username}", request.Username);
                return Ok(new { message = "Usuario registrado exitosamente", user });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar usuario: {Username}", request.Username);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // ============================================
        // LOGIN
        // ============================================
        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(UserDto request)
        {
            try
            {
                _logger.LogInformation("Intento de login para usuario: {Username}", request.Username);

                var result = await _authService.LoginAsync(request);
                if (result is null)
                {
                    _logger.LogWarning("Credenciales inválidas para: {Username}", request.Username);
                    return BadRequest(new { message = "Usuario o contraseña inválidos." });
                }

                // Guarda refresh token en cookie HttpOnly
                Response.Cookies.Append("refreshToken", result.RefreshToken, GetRefreshTokenCookieOptions());

                _logger.LogInformation("Login exitoso para: {Username}", request.Username);

                // Solo devuelve access token en el body
                return Ok(new { accessToken = result.AccessToken });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en login para: {Username}", request.Username);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // ============================================
        // REFRESH TOKEN
        // ============================================
        [HttpPost("refresh-token")]
        public async Task<ActionResult> RefreshToken()
        {
            try
            {
                // Lee el refresh token de la cookie
                var refreshToken = Request.Cookies["refreshToken"];

                if (string.IsNullOrEmpty(refreshToken))
                {
                    _logger.LogWarning("No se encontró refresh token en la cookie");
                    return Unauthorized(new { message = "No se encontró token de actualización." });
                }

                _logger.LogInformation("Intentando renovar token");

                // Valida el refresh token y obtiene el usuario
                var user = await _authService.GetUserByRefreshTokenAsync(refreshToken);
                if (user is null)
                {
                    _logger.LogWarning("Refresh token inválido o expirado");

                    // Elimina la cookie inválida
                    Response.Cookies.Delete("refreshToken", GetRefreshTokenCookieOptions());

                    return Unauthorized(new { message = "Token de actualización inválido o expirado." });
                }

                // Genera nuevos tokens
                var result = await _authService.CreateTokenResponse(user);
                if (result is null || string.IsNullOrEmpty(result.AccessToken) || string.IsNullOrEmpty(result.RefreshToken))
                {
                    _logger.LogError("Error al generar nuevos tokens para usuario: {UserId}", user.IdUser);
                    return StatusCode(500, new { message = "No se pudo generar nuevo token de acceso." });
                }

                // Actualiza la cookie con el nuevo refresh token
                Response.Cookies.Append("refreshToken", result.RefreshToken, GetRefreshTokenCookieOptions());

                _logger.LogInformation("Token renovado exitosamente para usuario: {UserId}", user.IdUser);

                return Ok(new { accessToken = result.AccessToken });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al renovar token");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // ============================================
        // LOGOUT
        // ============================================
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];

                if (!string.IsNullOrEmpty(refreshToken))
                {
                    _logger.LogInformation("Revocando refresh token");

                    // Revoca el token en la base de datos
                    var revoked = await _authService.RevokeRefreshTokenAsync(refreshToken);
                    if (!revoked)
                    {
                        _logger.LogWarning("No se pudo revocar el refresh token");
                    }
                }

                // Elimina la cookie (CRÍTICO: usar las mismas opciones)
                Response.Cookies.Delete("refreshToken", GetRefreshTokenCookieOptions());

                _logger.LogInformation("Logout exitoso");
                return Ok(new { message = "Sesión cerrada exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en logout");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        // ============================================
        // ENDPOINTS PROTEGIDOS (TESTING)
        // ============================================
        [Authorize]
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            var username = User.Identity?.Name;
            var roles = User.Claims
                .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            return Ok(new
            {
                username,
                roles,
                authenticated = true
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok(new { message = "Acceso autorizado como Admin" });
        }
    }
}