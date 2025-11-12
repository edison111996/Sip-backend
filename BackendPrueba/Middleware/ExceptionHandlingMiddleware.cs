using BackendPrueba.Exceptions;
using System.Net;
using System.Text.Json;

namespace BackendPrueba.Middleware
{
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

        // ✅ IMPORTANTE: Este método DEBE ser PUBLIC
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        // Este puede ser private
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            object response;
            int statusCode;

            switch (exception)
            {
                case UserNotFoundException userNotFound:
                    statusCode = (int)HttpStatusCode.NotFound;
                    response = new
                    {
                        message = userNotFound.Message,
                        errorCode = userNotFound.ErrorCode,
                        statusCode = statusCode
                    };
                    _logger.LogWarning("Usuario no encontrado: {Message}", userNotFound.Message);
                    break;

                case InvalidPasswordException invalidPassword:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    response = new
                    {
                        message = invalidPassword.Message,
                        errorCode = invalidPassword.ErrorCode,
                        statusCode = statusCode
                    };
                    _logger.LogWarning("Intento de login con contraseña incorrecta");
                    break;

                case AccountLockedException accountLocked:
                    statusCode = (int)HttpStatusCode.Forbidden;
                    response = new
                    {
                        message = accountLocked.Message,
                        errorCode = accountLocked.ErrorCode,
                        statusCode = statusCode
                    };
                    _logger.LogWarning("Intento de acceso a cuenta bloqueada");
                    break;

                case AuthenticationException authException:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    response = new
                    {
                        message = authException.Message,
                        errorCode = authException.ErrorCode,
                        statusCode = statusCode
                    };
                    _logger.LogWarning("Error de autenticación: {Message}", authException.Message);
                    break;

                default:
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

            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}