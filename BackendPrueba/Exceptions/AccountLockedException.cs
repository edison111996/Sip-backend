namespace BackendPrueba.Exceptions
{
    /// <summary>
    /// Excepción lanzada cuando la cuenta está bloqueada
    /// </summary>
    public class AccountLockedException : AuthenticationException
    {
        public AccountLockedException()
            : base("Cuenta bloqueada por múltiples intentos fallidos", "ACCOUNT_LOCKED")
        {
        }

        public AccountLockedException(string message)
            : base(message, "ACCOUNT_LOCKED")
        {
        }
    }
}
