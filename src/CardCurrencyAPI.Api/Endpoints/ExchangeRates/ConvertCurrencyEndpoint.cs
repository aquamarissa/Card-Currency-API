using CardCurrencyAPI.Application.DTOs;
using CardCurrencyAPI.Application.Features.ExchangeRates.Commands.ConvertCurrency;
using MediatR;

namespace CardCurrencyAPI.Api.Endpoints.ExchangeRates
{
    public static class ConvertCurrencyEndpoint
    {
        public static IEndpointRouteBuilder MapConvertCurrencyEndpoint(this IEndpointRouteBuilder builder)
        {
            builder.MapPost("/api/v1/exchange-rates/convert", async (
                ConvertCurrencyCommand request,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(request, cancellationToken);
                return Results.Ok(result);
            })
            .WithName("ConvertCurrency")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Convert currency using specified provider",
                Description = "Converts an amount from one currency to another using the specified exchange rate provider"
            })
            .Produces<CurrencyConversionDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

            return builder;
        }
    }
} 