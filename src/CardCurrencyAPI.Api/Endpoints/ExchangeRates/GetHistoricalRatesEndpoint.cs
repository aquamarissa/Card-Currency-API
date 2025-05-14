using CardCurrencyAPI.Application.DTOs;
using CardCurrencyAPI.Application.Features.ExchangeRates.Queries.GetHistoricalRates;
using MediatR;

namespace CardCurrencyAPI.Api.Endpoints.ExchangeRates
{
    public static class GetHistoricalRatesEndpoint
    {
        public static IEndpointRouteBuilder MapGetHistoricalRatesEndpoint(this IEndpointRouteBuilder builder)
        {
            builder.MapGet("/api/v1/exchange-rates/historical", async (
                string baseCurrency,
                DateTime startDate,
                DateTime endDate,
                int page,
                int pageSize,
                string? providerName,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetHistoricalRatesQuery
                {
                    BaseCurrency = baseCurrency,
                    StartDate = startDate,
                    EndDate = endDate,
                    Page = page,
                    PageSize = pageSize,
                    ProviderName = providerName ?? "Frankfurter"
                };

                var result = await mediator.Send(query, cancellationToken);
                return Results.Ok(result);
            })
            .WithName("GetHistoricalRates")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Get historical exchange rates",
                Description = "Retrieves historical exchange rates for a specified base currency and date range using the specified provider"
            })
            .Produces<HistoricalExchangeRateDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

            return builder;
        }
    }
} 