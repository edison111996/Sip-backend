using BackendPrueba.Exceptions;

namespace BackendPrueba.Middleware
{
    /// <summary>
    /// Excepción lanzada cuando la contraseña es incorrecta
    /// </summary>
    public class InvalidPasswordException : AuthenticationException
    {
        public InvalidPasswordException()
            : base("Usuario o contraseña incorrectos", "INVALID_PASSWORD")
        {
        }
    }
}
