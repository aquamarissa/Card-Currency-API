using CardCurrencyAPI.Application.DTOs;
using MediatR;

namespace CardCurrencyAPI.Application.Features.ExchangeRates.Queries.GetHistoricalRates
{
    public class GetHistoricalRatesQuery : IRequest<HistoricalExchangeRateDto>
    {
        public string BaseCurrency { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string ProviderName { get; set; } = "Frankfurter";
    }
} 