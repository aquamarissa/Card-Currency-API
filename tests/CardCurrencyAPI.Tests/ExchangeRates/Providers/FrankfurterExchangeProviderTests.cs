using CardCurrencyAPI.Domain.Exceptions;
using CardCurrencyAPI.Infrastructure.ExchangeRates.Clients;
using CardCurrencyAPI.Infrastructure.ExchangeRates.Configuration;
using CardCurrencyAPI.Infrastructure.ExchangeRates.Models;
using CardCurrencyAPI.Infrastructure.ExchangeRates.Providers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CardCurrencyAPI.Tests.ExchangeRates.Providers
{
    public class FrankfurterExchangeProviderTests
    {
        private readonly Mock<IFrankfurterApiClient> _clientMock;
        private readonly FrankfurterExchangeProvider _provider;

        public FrankfurterExchangeProviderTests()
        {
            _clientMock = new Mock<IFrankfurterApiClient>();

            var loggerMock = new Mock<ILogger<FrankfurterExchangeProvider>>(); 
            var cache = new MemoryCache(new MemoryCacheOptions());
            var options = Options.Create(new FrankfurterApiOptions());;

            _provider = new FrankfurterExchangeProvider(
                _clientMock.Object,
                cache,
                options,
                loggerMock.Object);
        }

        [Fact]
        public async Task GetLatestRatesAsync_WithValidBaseCurrency_ReturnsExchangeRate()
        {
            // Arrange
            var baseCurrency = "USD";
            var response = new FrankfurterLatestResponse
            {
                Base = baseCurrency,
                Date = DateTime.UtcNow,
                Rates = new Dictionary<string, decimal>
                {
                    { "EUR", 0.85m },
                    { "GBP", 0.73m }
                }
            };

            _clientMock.Setup(x => x.GetLatestRatesAsync(baseCurrency, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(response);

            // Act
            var result = await _provider.GetLatestRatesAsync(baseCurrency);

            // Assert
            Assert.Equal(baseCurrency, result.Base);
            Assert.Equal(response.Date, result.Date);
            Assert.Equal(response.Rates.Count, result.Rates.Count);
            Assert.Equal(response.Rates["EUR"], result.Rates["EUR"]);
            Assert.Equal(response.Rates["GBP"], result.Rates["GBP"]);
        }

        [Theory]
        [InlineData("TRY")]
        [InlineData("PLN")]
        [InlineData("THB")]
        [InlineData("MXN")]
        public async Task GetLatestRatesAsync_WithExcludedCurrency_ThrowsCurrencyNotSupportedException(string currency)
        {
            // Act & Assert
            await Assert.ThrowsAsync<CurrencyNotSupportedException>(
                () => _provider.GetLatestRatesAsync(currency));
        }

        [Fact]
        public async Task ConvertCurrencyAsync_WithValidCurrencies_ReturnsConvertedAmount()
        {
            // Arrange
            var fromCurrency = "USD";
            var toCurrency = "EUR";
            var amount = 100m;
            var rate = 0.85m;

            var response = new FrankfurterLatestResponse
            {
                Base = fromCurrency,
                Date = DateTime.UtcNow,
                Rates = new Dictionary<string, decimal>
                {
                    { toCurrency, rate }
                }
            };

            _clientMock.Setup(x => x.GetLatestRatesAsync(fromCurrency, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(response);

            // Act
            var result = await _provider.ConvertCurrencyAsync(amount, fromCurrency, toCurrency);

            // Assert
            Assert.Equal(amount * rate, result);
        }

        [Fact]
        public async Task GetHistoricalRatesAsync_WithValidParameters_ReturnsHistoricalRates()
        {
            // Arrange
            var baseCurrency = "USD";
            var startDate = DateTime.UtcNow.AddDays(-5);
            var endDate = DateTime.UtcNow;
            var page = 1;
            var pageSize = 2;

            var response = new FrankfurterHistoricalResponse
            {
                Base = baseCurrency,
                StartDate = startDate,
                EndDate = endDate,
                Rates = new Dictionary<DateTime, Dictionary<string, decimal>>
                {
                    {
                        startDate,
                        new Dictionary<string, decimal> { { "EUR", 0.85m } }
                    },
                    {
                        endDate,
                        new Dictionary<string, decimal> { { "EUR", 0.86m } }
                    }
                }
            };

            _clientMock.Setup(x => x.GetHistoricalRatesAsync(baseCurrency, startDate, endDate, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(response);

            // Act
            var result = await _provider.GetHistoricalRatesAsync(baseCurrency, startDate, endDate, page, pageSize);

            // Assert
            Assert.Equal(baseCurrency, result.Base);
            Assert.Equal(startDate, result.StartDate);
            Assert.Equal(endDate, result.EndDate);
            Assert.Equal(2, result.Rates.Count);
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(1, result.TotalPages);
            Assert.Equal(pageSize, result.PageSize);
            Assert.Equal(2, result.TotalItems);
        }

        [Fact]
        public async Task GetSupportedCurrenciesAsync_ReturnsFilteredCurrencies()
        {
            // Arrange
            var response = new FrankfurterCurrenciesResponse
            {
                { "USD", "US Dollar" },
                { "EUR", "Euro" },
                { "GBP", "British Pound" },
                { "TRY", "Turkish Lira" }, // Should be filtered out
                { "PLN", "Polish Zloty" }  // Should be filtered out
            };

            _clientMock.Setup(x => x.GetCurrenciesAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(response);

            // Act
            var result = await _provider.GetSupportedCurrenciesAsync();

            // Assert
            var currencies = new List<string>(result);
            Assert.Equal(3, currencies.Count);
            Assert.Contains("USD", currencies);
            Assert.Contains("EUR", currencies);
            Assert.Contains("GBP", currencies);
            Assert.DoesNotContain("TRY", currencies);
            Assert.DoesNotContain("PLN", currencies);
        }

        [Fact]
        public async Task GetLatestRatesAsync_UsesCachedResult_WhenAvailable()
        {
            // Arrange
            var baseCurrency = "USD";
            var response = new FrankfurterLatestResponse
            {
                Base = baseCurrency,
                Date = DateTime.UtcNow,
                Rates = new Dictionary<string, decimal>
                {
                    { "EUR", 0.85m }
                }
            };

            _clientMock.Setup(x => x.GetLatestRatesAsync(baseCurrency, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(response);

            // Act
            var firstResult = await _provider.GetLatestRatesAsync(baseCurrency);
            var secondResult = await _provider.GetLatestRatesAsync(baseCurrency);

            // Assert
            _clientMock.Verify(x => x.GetLatestRatesAsync(baseCurrency, It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal(firstResult.Base, secondResult.Base);
            Assert.Equal(firstResult.Date, secondResult.Date);
            Assert.Equal(firstResult.Rates["EUR"], secondResult.Rates["EUR"]);
        }
    }
} 