using CardCurrencyAPI.Infrastructure.ExchangeRates.Models;

namespace CardCurrencyAPI.Infrastructure.ExchangeRates.Clients;

public interface IFrankfurterApiClient
{
    Task<FrankfurterLatestResponse> GetLatestRatesAsync(string baseCurrency,
        CancellationToken cancellationToken = default);

    Task<FrankfurterHistoricalResponse> GetHistoricalRatesAsync(string baseCurrency,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);

    Task<FrankfurterCurrenciesResponse> GetCurrenciesAsync(CancellationToken cancellationToken = default);
}