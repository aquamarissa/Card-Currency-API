using CardCurrencyAPI.Application.Features.ExchangeRates.Commands.ConvertCurrency;
using CardCurrencyAPI.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace CardCurrencyAPI.Tests.Features.ExchangeRates.Commands
{
    public class ConvertCurrencyCommandHandlerTests
    {
        private readonly Mock<ICurrencyExchangeProviderFactory> _providerFactoryMock;
        private readonly Mock<ICurrencyExchangeProvider> _providerMock;
        private readonly Mock<ILogger<ConvertCurrencyCommandHandler>> _loggerMock;
        private readonly ConvertCurrencyCommandHandler _handler;

        public ConvertCurrencyCommandHandlerTests()
        {
            _providerFactoryMock = new Mock<ICurrencyExchangeProviderFactory>();
            _providerMock = new Mock<ICurrencyExchangeProvider>();
            _loggerMock = new Mock<ILogger<ConvertCurrencyCommandHandler>>();

            _providerFactoryMock.Setup(x => x.CreateProvider(It.IsAny<string>()))
                              .Returns(_providerMock.Object);

            _handler = new ConvertCurrencyCommandHandler(
                _providerFactoryMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidRequest_ReturnsConversionResult()
        {
            // Arrange
            var command = new ConvertCurrencyCommand
            {
                Amount = 100m,
                FromCurrency = "USD",
                ToCurrency = "EUR",
                ProviderName = "Frankfurter"
            };

            var rate = 0.85m;
            var date = DateTime.UtcNow;

            _providerMock.Setup(x => x.GetLatestRatesAsync(command.FromCurrency, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new Domain.Entities.ExchangeRate
                        {
                            Base = command.FromCurrency,
                            Date = date,
                            Rates = new() { { command.ToCurrency, rate } }
                        });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(command.Amount, result.Amount);
            Assert.Equal(command.FromCurrency, result.FromCurrency);
            Assert.Equal(command.ToCurrency, result.ToCurrency);
            Assert.Equal(command.Amount * rate, result.Result);
            Assert.Equal(rate, result.Rate);
            Assert.Equal(date, result.Date);
        }

        [Fact]
        public async Task Handle_WithInvalidCurrencyPair_ThrowsInvalidOperationException()
        {
            // Arrange
            var command = new ConvertCurrencyCommand
            {
                Amount = 100m,
                FromCurrency = "USD",
                ToCurrency = "INVALID",
                ProviderName = "Frankfurter"
            };

            _providerMock.Setup(x => x.GetLatestRatesAsync(command.FromCurrency, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new Domain.Entities.ExchangeRate
                        {
                            Base = command.FromCurrency,
                            Date = DateTime.UtcNow,
                            Rates = new() { { "EUR", 0.85m } }
                        });

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_CreatesProviderWithCorrectName()
        {
            // Arrange
            var command = new ConvertCurrencyCommand
            {
                Amount = 100m,
                FromCurrency = "USD",
                ToCurrency = "EUR",
                ProviderName = "CustomProvider"
            };

            _providerMock.Setup(x => x.GetLatestRatesAsync(command.FromCurrency, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new Domain.Entities.ExchangeRate
                        {
                            Base = command.FromCurrency,
                            Date = DateTime.UtcNow,
                            Rates = new() { { command.ToCurrency, 0.85m } }
                        });

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _providerFactoryMock.Verify(
                x => x.CreateProvider(command.ProviderName),
                Times.Once);
        }
    }
} 