using CardCurrencyAPI.Domain.Interfaces;
using CardCurrencyAPI.Infrastructure.ExchangeRates.Clients;
using CardCurrencyAPI.Infrastructure.ExchangeRates.Configuration;
using CardCurrencyAPI.Infrastructure.ExchangeRates.Factories;
using CardCurrencyAPI.Infrastructure.ExchangeRates.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace CardCurrencyAPI.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Correct section path that matches the appsettings.json structure
            var configSectionPath = "ExchangeRates:FrankfurterApi";
            
            services.Configure<FrankfurterApiOptions>(
                configuration.GetSection(configSectionPath));

            services.AddMemoryCache();

            services.AddHttpClient<IFrankfurterApiClient, FrankfurterApiClient>()
                .AddPolicyHandler((provider, _) =>
                {
                    var options = configuration
                        .GetSection(configSectionPath)
                        .Get<FrankfurterApiOptions>();

                    return HttpPolicyExtensions
                        .HandleTransientHttpError()
                        .WaitAndRetryAsync(
                            options?.RetryCount ?? 3,
                            retryAttempt => TimeSpan.FromMilliseconds(
                                (options?.RetryDelayMilliseconds ?? 500) * Math.Pow(2, retryAttempt - 1)));
                })
                .AddPolicyHandler((provider, _) =>
                {
                    var options = configuration
                        .GetSection(configSectionPath)
                        .Get<FrankfurterApiOptions>();

                    return HttpPolicyExtensions
                        .HandleTransientHttpError()
                        .CircuitBreakerAsync(
                            options?.CircuitBreakerFailuresBeforeBreaking ?? 3,
                            TimeSpan.FromSeconds(options?.CircuitBreakerDurationOfBreakSeconds ?? 60));
                });

            services.AddScoped<FrankfurterExchangeProvider>();
            services.AddScoped<ICurrencyExchangeProviderFactory, CurrencyExchangeProviderFactory>();

            return services;
        }
    }
} 