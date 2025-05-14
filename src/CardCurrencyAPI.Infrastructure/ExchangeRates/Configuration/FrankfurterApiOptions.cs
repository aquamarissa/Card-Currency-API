namespace CardCurrencyAPI.Infrastructure.ExchangeRates.Configuration
{
    public class FrankfurterApiOptions
    {
        public const string SectionName = "ExchangeRates:FrankfurterApi";
        
        public string BaseUrl { get; set; } = "https://api.frankfurter.app";
        public int CacheExpirationMinutes { get; set; } = 30;
        public int RetryCount { get; set; } = 3;
        public int RetryDelayMilliseconds { get; set; } = 500;
        public int TimeoutSeconds { get; set; } = 30;
        public int CircuitBreakerFailuresBeforeBreaking { get; set; } = 3;
        public int CircuitBreakerDurationOfBreakSeconds { get; set; } = 60;
    }
} 