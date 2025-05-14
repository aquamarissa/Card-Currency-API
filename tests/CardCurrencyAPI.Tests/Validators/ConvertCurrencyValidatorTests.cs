using CardCurrencyAPI.Application.Features.ExchangeRates.Commands.ConvertCurrency;
using CardCurrencyAPI.Application.Validators;
using FluentValidation.TestHelper;

namespace CardCurrencyAPI.Tests.Validators
{
    public class ConvertCurrencyValidatorTests
    {
        private readonly ConvertCurrencyValidator _validator;

        public ConvertCurrencyValidatorTests()
        {
            _validator = new ConvertCurrencyValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Amount_ShouldHaveError_WhenNotGreaterThanZero(decimal amount)
        {
            // Arrange
            var command = new ConvertCurrencyCommand { Amount = amount };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Amount)
                 .WithErrorMessage("Amount must be greater than zero");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(" ")]
        public void FromCurrency_ShouldHaveError_WhenEmpty(string currency)
        {
            // Arrange
            var command = new ConvertCurrencyCommand { FromCurrency = currency };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.FromCurrency)
                 .WithErrorMessage("From currency is required");
        }

        [Theory]
        [InlineData("TRY")]
        [InlineData("PLN")]
        [InlineData("THB")]
        [InlineData("MXN")]
        public void FromCurrency_ShouldHaveError_WhenExcludedCurrency(string currency)
        {
            // Arrange
            var command = new ConvertCurrencyCommand { FromCurrency = currency };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.FromCurrency)
                 .WithErrorMessage($"Currency '{currency}' is not supported. TRY, PLN, THB, and MXN are excluded currencies.");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(" ")]
        public void ToCurrency_ShouldHaveError_WhenEmpty(string currency)
        {
            // Arrange
            var command = new ConvertCurrencyCommand { ToCurrency = currency };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ToCurrency)
                 .WithErrorMessage("To currency is required");
        }

        [Theory]
        [InlineData("TRY")]
        [InlineData("PLN")]
        [InlineData("THB")]
        [InlineData("MXN")]
        public void ToCurrency_ShouldHaveError_WhenExcludedCurrency(string currency)
        {
            // Arrange
            var command = new ConvertCurrencyCommand { ToCurrency = currency };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ToCurrency)
                 .WithErrorMessage($"Currency '{currency}' is not supported. TRY, PLN, THB, and MXN are excluded currencies.");
        }

        [Fact]
        public void ShouldHaveError_WhenFromAndToCurrenciesAreSame()
        {
            // Arrange
            var command = new ConvertCurrencyCommand
            {
                FromCurrency = "USD",
                ToCurrency = "USD"
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x)
                 .WithErrorMessage("From and To currencies must be different");
        }

        [Fact]
        public void ShouldNotHaveError_WhenRequestIsValid()
        {
            // Arrange
            var command = new ConvertCurrencyCommand
            {
                Amount = 100m,
                FromCurrency = "USD",
                ToCurrency = "EUR"
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
} 