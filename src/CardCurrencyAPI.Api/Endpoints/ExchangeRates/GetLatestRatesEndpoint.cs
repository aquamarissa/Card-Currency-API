using CardCurrencyAPI.Application.DTOs;
using CardCurrencyAPI.Application.Features.ExchangeRates.Queries.GetLatestRates;
using MediatR;

namespace CardCurrencyAPI.Api.Endpoints.ExchangeRates
{
    public static class GetLatestRatesEndpoint
    {
        public static IEndpointRouteBuilder MapGetLatestRatesEndpoint(this IEndpointRouteBuilder builder)
        {
            builder.MapGet("/api/v1/exchange-rates/latest", async (
                string baseCurrency,
                string? providerName,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetLatestRatesQuery
                {
                    BaseCurrency = baseCurrency,
                    ProviderName = providerName ?? "Frankfurter"
                };

                var result = await mediator.Send(query, cancellationToken);
                return Results.Ok(result);
            })
            .WithName("GetLatestRates")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Get latest exchange rates",
                Description = "Retrieves the latest exchange rates for a specified base currency using the specified provider"
            })
            .Produces<ExchangeRateDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

            return builder;
        }
    }
} 