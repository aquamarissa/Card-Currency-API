using CardCurrencyAPI.Domain.Entities;
using CardCurrencyAPI.Domain.Exceptions;
using CardCurrencyAPI.Domain.Interfaces;
using CardCurrencyAPI.Infrastructure.ExchangeRates.Clients;
using CardCurrencyAPI.Infrastructure.ExchangeRates.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CardCurrencyAPI.Infrastructure.ExchangeRates.Providers
{
    public class FrankfurterExchangeProvider(
        IFrankfurterApiClient client,
        IMemoryCache cache,
        IOptions<FrankfurterApiOptions> options,
        ILogger<FrankfurterExchangeProvider> logger)
            : ICurrencyExchangeProvider
    {
        private readonly HashSet<string> _excludedCurrencies = new() { "TRY", "PLN", "THB", "MXN" };

        public async Task<ExchangeRate> GetLatestRatesAsync(string baseCurrency, CancellationToken cancellationToken = default)
        {
            ValidateCurrency(baseCurrency);
            
            var cacheKey = $"latest-rates-{baseCurrency}";
            
            if (cache.TryGetValue(cacheKey, out ExchangeRate cachedRates))
            {
                logger.LogInformation("Retrieved latest rates for {BaseCurrency} from cache", baseCurrency);
                return cachedRates;
            }

            logger.LogInformation("Getting latest rates for {BaseCurrency} from Frankfurter API", baseCurrency);
            var response = await client.GetLatestRatesAsync(baseCurrency, cancellationToken);
            
            var filteredRates = response.Rates
                .Where(r => !_excludedCurrencies.Contains(r.Key))
                .ToDictionary(r => r.Key, r => r.Value);
            
            var result = new ExchangeRate
            {
                Base = response.Base,
                Date = response.Date,
                Rates = filteredRates
            };
            
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(options.Value.CacheExpirationMinutes));
            
            cache.Set(cacheKey, result, cacheEntryOptions);
            
            return result;
        }

        public async Task<decimal> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency, CancellationToken cancellationToken = default)
        {
            ValidateCurrency(fromCurrency);
            ValidateCurrency(toCurrency);
            
            if (fromCurrency == toCurrency)
            {
                return amount;
            }
            
            var latestRates = await GetLatestRatesAsync(fromCurrency, cancellationToken);
            
            if (!latestRates.Rates.TryGetValue(toCurrency, out var rate))
            {
                throw new InvalidOperationException($"Cannot find exchange rate from {fromCurrency} to {toCurrency}");
            }
            
            return amount * rate;
        }

        public async Task<HistoricalExchangeRate> GetHistoricalRatesAsync(
            string baseCurrency, 
            DateTime startDate, 
            DateTime endDate, 
            int page, 
            int pageSize, 
            CancellationToken cancellationToken = default)
        {
            ValidateCurrency(baseCurrency);
            
            var cacheKey = $"historical-rates-{baseCurrency}-{startDate:yyyy-MM-dd}-{endDate:yyyy-MM-dd}-{page}-{pageSize}";
            
            if (cache.TryGetValue(cacheKey, out HistoricalExchangeRate cachedRates))
            {
                logger.LogInformation("Retrieved historical rates for {BaseCurrency} from {StartDate} to {EndDate} (page {Page}) from cache", 
                    baseCurrency, startDate, endDate, page);
                return cachedRates;
            }

            logger.LogInformation("Getting historical rates for {BaseCurrency} from {StartDate} to {EndDate} from Frankfurter API", 
                baseCurrency, startDate, endDate);
                
            var response = await client.GetHistoricalRatesAsync(baseCurrency, startDate, endDate, cancellationToken);
            
            var filteredRates = new Dictionary<DateTime, Dictionary<string, decimal>>();
            foreach (var (date, rates) in response.Rates)
            {
                filteredRates[date] = rates
                    .Where(r => !_excludedCurrencies.Contains(r.Key))
                    .ToDictionary(r => r.Key, r => r.Value);
            }
            
            var dateKeys = filteredRates.Keys.OrderBy(d => d).ToList();
            var totalItems = dateKeys.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            page = Math.Max(1, Math.Min(page, totalPages));
            
            var pagedDates = dateKeys
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            var pagedRates = new Dictionary<DateTime, Dictionary<string, decimal>>();
            foreach (var date in pagedDates)
            {
                pagedRates[date] = filteredRates[date];
            }
            
            var result = new HistoricalExchangeRate
            {
                Base = response.Base,
                StartDate = startDate,
                EndDate = endDate,
                Rates = pagedRates,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalItems = totalItems
            };
            
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(options.Value.CacheExpirationMinutes));
            
            cache.Set(cacheKey, result, cacheEntryOptions);
            
            return result;
        }

        public async Task<IEnumerable<string>> GetSupportedCurrenciesAsync(CancellationToken cancellationToken = default)
        {
            var cacheKey = "supported-currencies";
            
            if (cache.TryGetValue(cacheKey, out IEnumerable<string> cachedCurrencies))
            {
                logger.LogInformation("Retrieved supported currencies from cache");
                return cachedCurrencies;
            }

            logger.LogInformation("Getting supported currencies from Frankfurter API");
            var response = await client.GetCurrenciesAsync(cancellationToken);
            
            var supportedCurrencies = response.Keys
                .Where(currency => !_excludedCurrencies.Contains(currency))
                .ToList();
            
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(options.Value.CacheExpirationMinutes));
            
            cache.Set(cacheKey, supportedCurrencies, cacheEntryOptions);
            
            return supportedCurrencies;
        }

        private void ValidateCurrency(string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
            {
                throw new ArgumentException("Currency code cannot be empty", nameof(currency));
            }
            
            if (_excludedCurrencies.Contains(currency.ToUpper()))
            {
                throw new CurrencyNotSupportedException(currency.ToUpper());
            }
        }
    }
} 