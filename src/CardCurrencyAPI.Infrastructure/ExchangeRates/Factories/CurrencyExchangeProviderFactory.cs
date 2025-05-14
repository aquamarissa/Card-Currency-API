using CardCurrencyAPI.Application.Common.Exceptions;
using CardCurrencyAPI.Domain.Interfaces;
using CardCurrencyAPI.Infrastructure.ExchangeRates.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace CardCurrencyAPI.Infrastructure.ExchangeRates.Factories
{
    public class CurrencyExchangeProviderFactory(IServiceProvider serviceProvider): ICurrencyExchangeProviderFactory
    {
        public ICurrencyExchangeProvider CreateProvider(string providerName)
        {
            return providerName.ToLowerInvariant() switch
            {
                "frankfurter" => serviceProvider.GetRequiredService<FrankfurterExchangeProvider>(),
                _ => throw new NotFoundException(providerName)
            };
        }

        public ICurrencyExchangeProvider CreateDefaultProvider()
        {
            return serviceProvider.GetRequiredService<FrankfurterExchangeProvider>();
        }
    }
} 