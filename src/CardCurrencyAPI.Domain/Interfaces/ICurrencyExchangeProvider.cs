using CardCurrencyAPI.Domain.Entities;

namespace CardCurrencyAPI.Domain.Interfaces
{
    public interface ICurrencyExchangeProvider
    {
        /// <summary>
        /// Gets the latest exchange rates for a specified base currency
        /// </summary>
        Task<ExchangeRate> GetLatestRatesAsync(string baseCurrency, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Converts an amount from one currency to another
        /// </summary>
        Task<decimal> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets historical exchange rates for a period with pagination
        /// </summary>
        Task<HistoricalExchangeRate> GetHistoricalRatesAsync(
            string baseCurrency, 
            DateTime startDate, 
            DateTime endDate, 
            int page, 
            int pageSize, 
            CancellationToken cancellationToken = default);
            
        /// <summary>
        /// Gets a list of supported currencies
        /// </summary>
        Task<IEnumerable<string>> GetSupportedCurrenciesAsync(CancellationToken cancellationToken = default);
    }
} 