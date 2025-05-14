using CardCurrencyAPI.Application.DTOs;
using CardCurrencyAPI.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CardCurrencyAPI.Application.Features.ExchangeRates.Queries.GetLatestRates
{
    public class GetLatestRatesQueryHandler(
        ICurrencyExchangeProviderFactory providerFactory,
        ILogger<GetLatestRatesQueryHandler> logger)
            : IRequestHandler<GetLatestRatesQuery, ExchangeRateDto>
    {
        public async Task<ExchangeRateDto> Handle(GetLatestRatesQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Getting latest rates for base currency {BaseCurrency} using provider {ProviderName}", 
                request.BaseCurrency, request.ProviderName);
            
            var provider = providerFactory.CreateProvider(request.ProviderName);
            var result = await provider.GetLatestRatesAsync(request.BaseCurrency, cancellationToken);
            
            return new ExchangeRateDto
            {
                Base = result.Base,
                Date = result.Date,
                Rates = result.Rates
            };
        }
    }
} 