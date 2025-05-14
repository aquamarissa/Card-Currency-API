using CardCurrencyAPI.Application.DTOs;
using CardCurrencyAPI.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CardCurrencyAPI.Application.Features.ExchangeRates.Commands.ConvertCurrency
{
    public class ConvertCurrencyCommandHandler(
        ICurrencyExchangeProviderFactory providerFactory,
        ILogger<ConvertCurrencyCommandHandler> logger)
            : IRequestHandler<ConvertCurrencyCommand, CurrencyConversionDto>
    {
        public async Task<CurrencyConversionDto> Handle(ConvertCurrencyCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Converting {Amount} from {FromCurrency} to {ToCurrency} using provider {ProviderName}", 
                request.Amount, request.FromCurrency, request.ToCurrency, request.ProviderName);
            
            var provider = providerFactory.CreateProvider(request.ProviderName);
            
            var latestRates = await provider.GetLatestRatesAsync(request.FromCurrency, cancellationToken);
            
            if (!latestRates.Rates.TryGetValue(request.ToCurrency, out var rate))
            {
                throw new InvalidOperationException($"Cannot find exchange rate from {request.FromCurrency} to {request.ToCurrency}");
            }
            
            var result = request.Amount * rate;
            
            return new CurrencyConversionDto
            {
                Amount = request.Amount,
                FromCurrency = request.FromCurrency,
                ToCurrency = request.ToCurrency,
                Result = result,
                Rate = rate,
                Date = latestRates.Date
            };
        }
    }
} 