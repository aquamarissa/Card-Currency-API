using CardCurrencyAPI.Application.DTOs;
using CardCurrencyAPI.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CardCurrencyAPI.Application.Features.ExchangeRates.Queries.GetHistoricalRates
{
    public class GetHistoricalRatesQueryHandler(
        ICurrencyExchangeProviderFactory providerFactory,
        ILogger<GetHistoricalRatesQueryHandler> logger)
            : IRequestHandler<GetHistoricalRatesQuery, HistoricalExchangeRateDto>
    {
        public async Task<HistoricalExchangeRateDto> Handle(GetHistoricalRatesQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "Getting historical rates for base currency {BaseCurrency} from {StartDate} to {EndDate} (Page {Page}, PageSize {PageSize}) using provider {ProviderName}", 
                request.BaseCurrency, request.StartDate, request.EndDate, request.Page, request.PageSize, request.ProviderName);
            
            var provider = providerFactory.CreateProvider(request.ProviderName);
            var result = await provider.GetHistoricalRatesAsync(
                request.BaseCurrency, 
                request.StartDate, 
                request.EndDate, 
                request.Page, 
                request.PageSize, 
                cancellationToken);
            
            var dto = new HistoricalExchangeRateDto
            {
                Base = result.Base,
                StartDate = result.StartDate,
                EndDate = result.EndDate,
                Rates = result.Rates,
                Pagination = new PaginationMetadata
                {
                    CurrentPage = result.CurrentPage,
                    TotalPages = result.TotalPages,
                    PageSize = result.PageSize,
                    TotalItems = result.TotalItems
                }
            };
            
            return dto;
        }
    }
} 