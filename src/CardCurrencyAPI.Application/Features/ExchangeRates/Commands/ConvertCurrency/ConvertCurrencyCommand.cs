using CardCurrencyAPI.Application.DTOs;
using MediatR;

namespace CardCurrencyAPI.Application.Features.ExchangeRates.Commands.ConvertCurrency
{
    public class ConvertCurrencyCommand : IRequest<CurrencyConversionDto>
    {
        public decimal Amount { get; set; }
        public string FromCurrency { get; set; } = string.Empty;
        public string ToCurrency { get; set; } = string.Empty;
        public string ProviderName { get; set; } = "Frankfurter";
    }
} 