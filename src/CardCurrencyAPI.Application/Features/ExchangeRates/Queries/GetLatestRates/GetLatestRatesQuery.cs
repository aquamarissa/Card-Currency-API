using CardCurrencyAPI.Application.DTOs;
using MediatR;

namespace CardCurrencyAPI.Application.Features.ExchangeRates.Queries.GetLatestRates
{
    public class GetLatestRatesQuery : IRequest<ExchangeRateDto>
    {
        public string BaseCurrency { get; set; } = string.Empty;
        public string ProviderName { get; set; } = "Frankfurter";
    }
} 