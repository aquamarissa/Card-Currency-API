namespace CardCurrencyAPI.Application.Common.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException()
            : base("User is not authorized to perform this action.")
        {
        }

        public UnauthorizedException(string message)
            : base(message)
        {
        }

        public UnauthorizedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
} 