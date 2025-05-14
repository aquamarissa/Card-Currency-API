namespace CardCurrencyAPI.Application.Common.Exceptions
{
    public class RateLimitExceededException : Exception
    {
        public RateLimitExceededException()
            : base("Rate limit has been exceeded. Please try again later.")
        {
        }

        public RateLimitExceededException(string message)
            : base(message)
        {
        }

        public RateLimitExceededException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
} 