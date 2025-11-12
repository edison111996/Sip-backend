using BackendPrueba.Exceptions;

namespace BackendPrueba.Middleware
{
    /// <summary>
    /// Excepción lanzada cuando el usuario no existe
    /// </summary>
    public class UserNotFoundException : AuthenticationException
    {
        public UserNotFoundException()
            : base("Usuario no encontrado", "USER_NOT_FOUND")
        {
        }

        public UserNotFoundException(string username)
            : base($"El usuario '{username}' no existe", "USER_NOT_FOUND")
        {
        }
    }
}
