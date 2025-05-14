namespace CardCurrencyAPI.Api.Endpoints.ExchangeRates
{
    public static class ExchangeRatesEndpointExtensions
    {
        public static IEndpointRouteBuilder MapExchangeRatesEndpoints(this IEndpointRouteBuilder builder)
        {
            builder.MapConvertCurrencyEndpoint()
                   .MapGetLatestRatesEndpoint()
                   .MapGetHistoricalRatesEndpoint();

            return builder;
        }
    }
} 