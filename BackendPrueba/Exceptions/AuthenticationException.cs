namespace BackendPrueba.Exceptions
{
        /// <summary>
        /// Excepción base para errores de autenticación
        /// </summary>
        public class AuthenticationException : Exception
        {
            public string ErrorCode { get; }

            public AuthenticationException(string message, string errorCode)
                : base(message)
            {
                ErrorCode = errorCode;
            }
        }
    
}
