using CardCurrencyAPI.Application.Features.ExchangeRates.Commands.ConvertCurrency;
using FluentValidation;

namespace CardCurrencyAPI.Application.Validators
{
    public class ConvertCurrencyValidator: AbstractValidator<ConvertCurrencyCommand>
    {
        private readonly HashSet<string> _excludedCurrencies = new()
        {
            "TRY", "PLN", "THB", "MXN"
        };

        public ConvertCurrencyValidator()
        {
            RuleFor(x => x.Amount)
                    .GreaterThan(0).WithMessage("Amount must be greater than zero");

            RuleFor(x => x.FromCurrency)
                    .Cascade(CascadeMode.Stop)
                    .Must(currency => !string.IsNullOrWhiteSpace(currency)).WithMessage("From currency is required")
                    .Must(currency => !_excludedCurrencies.Contains(currency.ToUpper()))
                    .WithMessage(x
                            => $"Currency '{x.FromCurrency}' is not supported. TRY, PLN, THB, and MXN are excluded currencies.");

            RuleFor(x => x.ToCurrency)
                    .Cascade(CascadeMode.Stop)
                    .Must(currency => !string.IsNullOrWhiteSpace(currency)).WithMessage("To currency is required")
                    .Must(currency => !_excludedCurrencies.Contains(currency.ToUpper()))
                    .WithMessage(x => $"Currency '{x.ToCurrency}' is not supported. TRY, PLN, THB, and MXN are excluded currencies.");

            RuleFor(x => x)
                    .Must(x => x.FromCurrency != x.ToCurrency)
                    .WithMessage("From and To currencies must be different");
        }
    }
}