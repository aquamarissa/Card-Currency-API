using CardCurrencyAPI.Application.Features.ExchangeRates.Queries.GetHistoricalRates;
using FluentValidation;

namespace CardCurrencyAPI.Application.Validators
{
    public class GetHistoricalRatesValidator : AbstractValidator<GetHistoricalRatesQuery>
    {
        private readonly HashSet<string> _excludedCurrencies = new() { "TRY", "PLN", "THB", "MXN" };
        
        public GetHistoricalRatesValidator()
        {
            RuleFor(x => x.BaseCurrency)
                    .Cascade(CascadeMode.Stop)
                    .Must(currency => !string.IsNullOrWhiteSpace(currency)).WithMessage("Base currency is required")
                .Must(currency => !_excludedCurrencies.Contains(currency.ToUpper()))
                .WithMessage(x => $"Currency '{x.BaseCurrency}' is not supported. TRY, PLN, THB, and MXN are excluded currencies.");
                
            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required")
                .Must(BeValidDate).WithMessage("Start date must be a valid date");
                
            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required")
                .Must(BeValidDate).WithMessage("End date must be a valid date")
                .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("End date must be greater than or equal to start date");
                
            RuleFor(x => x.Page)
                .GreaterThan(0).WithMessage("Page must be greater than 0");
                
            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");
                
            RuleFor(x => x)
                .Must(x => (x.EndDate - x.StartDate).TotalDays <= 365)
                .WithMessage("Date range cannot exceed 365 days");
        }
        
        private bool BeValidDate(DateTime date)
        {
            return date != default && date <= DateTime.UtcNow;
        }
    }
} 