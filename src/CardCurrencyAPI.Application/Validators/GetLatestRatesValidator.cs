using CardCurrencyAPI.Application.Features.ExchangeRates.Queries.GetLatestRates;
using FluentValidation;

namespace CardCurrencyAPI.Application.Validators
{
    public class GetLatestRatesValidator : AbstractValidator<GetLatestRatesQuery>
    {
        private readonly HashSet<string> _excludedCurrencies = new() { "TRY", "PLN", "THB", "MXN" };
        
        public GetLatestRatesValidator()
        {
            RuleFor(x => x.BaseCurrency)
                .NotEmpty().WithMessage("Base currency is required")
                .Must(currency => !_excludedCurrencies.Contains(currency.ToUpper()))
                .WithMessage(x => $"Currency '{x.BaseCurrency}' is not supported. TRY, PLN, THB, and MXN are excluded currencies.");
        }
    }
} 