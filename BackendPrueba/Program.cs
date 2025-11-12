using BackendPrueba.Data;
using BackendPrueba.Services;
using BackendPrueba.Services.ModuleServices;
using BackendPrueba.Services.PermissionService;
using BackendPrueba.Services.RolePermissionsService;
using BackendPrueba.Services.Roles;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// VALIDAR Y OBTENER CONFIGURACIÓN JWT
// ============================================
var jwtKey = builder.Configuration["AppSettings:Token"]
    ?? throw new InvalidOperationException("JWT Token no está configurado en appsettings.json");

var issuer = builder.Configuration["AppSettings:Issuer"]
    ?? throw new InvalidOperationException("JWT Issuer no está configurado");

var audience = builder.Configuration["AppSettings:Audience"]
    ?? throw new InvalidOperationException("JWT Audience no está configurado");

// ✅ Validar longitud mínima de la clave (256 bits = 32 caracteres para HS256)
if (jwtKey.Length < 32)
{
    throw new InvalidOperationException(
        $"La clave JWT es muy corta ({jwtKey.Length} caracteres). Debe tener al menos 32 caracteres para HS256.");
}

// ✅ Crear y cachear la clave de firma (evita recrearla en cada request)
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

// ============================================
// SERVICIOS
// ============================================
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddAutoMapper(typeof(Program));

// ✅ Registrar la clave como Singleton para reutilizarla en toda la app
builder.Services.AddSingleton(signingKey);

// ============================================
// RATE LIMITING
// ============================================
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var path = httpContext.Request.Path.Value?.ToLower() ?? "";

        // Sin límite para endpoints de autenticación
        if (path.StartsWith("/api/auth"))
        {
            return RateLimitPartition.GetNoLimiter<string>("auth");
        }

        // Límite para otros endpoints
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 5
            });
    });
    options.RejectionStatusCode = 429;
});

// ============================================
// BASE DE DATOS
// ============================================
builder.Services.AddDbContext<VideoGameDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ============================================
// AUTENTICACIÓN JWT - MEJORADA
// ============================================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Validación del Issuer (quién emitió el token)
            ValidateIssuer = true,
            ValidIssuer = issuer,

            // Validación del Audience (para quién es el token)
            ValidateAudience = true,
            ValidAudience = audience,

            // Validación de expiración
            ValidateLifetime = true,
            RequireExpirationTime = true, // ✅ Requiere el claim 'exp'
            ClockSkew = TimeSpan.Zero,    // ✅ Sin tolerancia de tiempo

            // Validación de la firma
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey, // ✅ Usar clave cacheada
            RequireSignedTokens = true,    // ✅ Requiere que el token esté firmado

            // ✅ SEGURIDAD: Solo permitir algoritmo HMAC SHA-256
            ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 }
        };

        // ============================================
        // EVENTOS DEL MIDDLEWARE JWT
        // ============================================
        options.Events = new JwtBearerEvents
        {
            // ❌ ELIMINADO OnMessageReceived - es redundante
            // El middleware ya lee automáticamente del header Authorization

            // Cuando falla la autenticación
            OnAuthenticationFailed = context =>
            {
                // Detectar token expirado
                if (context.Exception is SecurityTokenExpiredException)
                {
                    context.Response.Headers.Append("Token-Expired", "true");

                    // ✅ Logear en desarrollo
                    if (builder.Environment.IsDevelopment())
                    {
                        Console.WriteLine($"⚠️ Token expirado: {context.Exception.Message}");
                    }
                }
                else if (builder.Environment.IsDevelopment())
                {
                    // ✅ Logear otros errores solo en desarrollo
                    Console.WriteLine($"❌ Error de autenticación: {context.Exception.GetType().Name} - {context.Exception.Message}");
                }

                return Task.CompletedTask;
            },

            // Personalizar respuesta 401 Unauthorized
            OnChallenge = context =>
            {
                context.HandleResponse(); // Prevenir respuesta por defecto

                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                var errorMessage = "No autorizado";

                // ✅ En desarrollo, dar más detalles
                if (builder.Environment.IsDevelopment() && context.AuthenticateFailure != null)
                {
                    errorMessage = context.AuthenticateFailure.Message;
                }

                var result = JsonSerializer.Serialize(new
                {
                    success = false,
                    message = errorMessage,
                    statusCode = 401
                });

                return context.Response.WriteAsync(result);
            },

            // ✅ Opcional: Validaciones adicionales después de validar el token
            OnTokenValidated = context =>
            {
                // Aquí puedes agregar lógica personalizada:
                // - Validar si el usuario sigue activo en la DB
                // - Verificar roles o permisos adicionales
                // - Revocar tokens comprometidos

                if (builder.Environment.IsDevelopment())
                {
                    var userId = context.Principal?.FindFirst("id")?.Value;
                    var username = context.Principal?.Identity?.Name;
                    Console.WriteLine($"✅ Token validado para usuario: {username} (ID: {userId})");
                }

                return Task.CompletedTask;
            }
        };
    });

// ============================================
// CORS - CRÍTICO PARA COOKIES
// ============================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:5173") // 👈 Tu frontend
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // 👈 CRÍTICO para cookies HttpOnly
    });
});

// ============================================
// SERVICIOS PERSONALIZADOS
// ============================================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RolesService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();
builder.Services.AddScoped<IModuleService, ModuleService>();

var app = builder.Build();

app.UseMiddleware<BackendPrueba.Middleware.ExceptionHandlingMiddleware>();

// ============================================
// MIDDLEWARE PIPELINE
// ============================================
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// ✅ ORDEN CRÍTICO: CORS debe ir ANTES de Authentication
app.UseCors("AllowFrontend");

app.UseRateLimiter();

// ✅ ORDEN CRÍTICO: Authentication antes de Authorization
app.UseAuthentication(); // 🔐 Verifica el token
app.UseAuthorization();  // 🔒 Verifica permisos

app.MapControllers();

app.Run();