using BackendPrueba.Middleware;
using System.Net;
using System.Text.Json;

namespace BackendPrueba.Exceptions
{
    /// <summary>
    /// Middleware para capturar y manejar excepciones globalmente
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Continuar con la siguiente middleware
                await _next(context);
            }
            catch (Exception ex)
            {
                // Capturar cualquier excepción y manejarla
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Configurar respuesta como JSON
            context.Response.ContentType = "application/json";

            // Objeto de respuesta por defecto
            object response;
            int statusCode;

            // Manejar diferentes tipos de excepciones
            switch (exception)
            {
                case UserNotFoundException userNotFound:
                    statusCode = (int)HttpStatusCode.NotFound; // 404
                    response = new
                    {
                        message = userNotFound.Message,
                        errorCode = userNotFound.ErrorCode,
                        statusCode = statusCode
                    };
                    _logger.LogWarning("Usuario no encontrado: {Message}", userNotFound.Message);
                    break;

                case InvalidPasswordException invalidPassword:
                    statusCode = (int)HttpStatusCode.Unauthorized; // 401
                    response = new
                    {
                        message = invalidPassword.Message,
                        errorCode = invalidPassword.ErrorCode,
                        statusCode = statusCode
                    };
                    _logger.LogWarning("Intento de login con contraseña incorrecta");
                    break;

                case AccountLockedException accountLocked:
                    statusCode = (int)HttpStatusCode.Forbidden; // 403
                    response = new
                    {
                        message = accountLocked.Message,
                        errorCode = accountLocked.ErrorCode,
                        statusCode = statusCode
                    };
                    _logger.LogWarning("Intento de acceso a cuenta bloqueada");
                    break;

                case AuthenticationException authException:
                    statusCode = (int)HttpStatusCode.Unauthorized; // 401
                    response = new
                    {
                        message = authException.Message,
                        errorCode = authException.ErrorCode,
                        statusCode = statusCode
                    };
                    _logger.LogWarning("Error de autenticación: {Message}", authException.Message);
                    break;

                default:
                    // Error no manejado - 500
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    response = new
                    {
                        message = "Ha ocurrido un error interno. Por favor intenta más tarde.",
                        errorCode = "INTERNAL_ERROR",
                        statusCode = statusCode
                    };
                    _logger.LogError(exception, "Error interno no manejado: {Message}", exception.Message);
                    break;
            }

            context.Response.StatusCode = statusCode;

            // Serializar y enviar la respuesta
            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
