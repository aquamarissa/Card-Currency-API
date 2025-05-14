using CardCurrencyAPI.Application.Features.ExchangeRates.Queries.GetHistoricalRates;
using CardCurrencyAPI.Application.Validators;
using FluentValidation.TestHelper;

namespace CardCurrencyAPI.Tests.Validators
{
    public class GetHistoricalRatesValidatorTests
    {
        private readonly GetHistoricalRatesValidator _validator;

        public GetHistoricalRatesValidatorTests()
        {
            _validator = new GetHistoricalRatesValidator();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(" ")]
        public void BaseCurrency_ShouldHaveError_WhenEmpty(string currency)
        {
            // Arrange
            var query = new GetHistoricalRatesQuery { BaseCurrency = currency };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.BaseCurrency)
                 .WithErrorMessage("Base currency is required");
        }

        [Theory]
        [InlineData("TRY")]
        [InlineData("PLN")]
        [InlineData("THB")]
        [InlineData("MXN")]
        public void BaseCurrency_ShouldHaveError_WhenExcludedCurrency(string currency)
        {
            // Arrange
            var query = new GetHistoricalRatesQuery { BaseCurrency = currency };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.BaseCurrency)
                 .WithErrorMessage($"Currency '{currency}' is not supported. TRY, PLN, THB, and MXN are excluded currencies.");
        }

        [Fact]
        public void StartDate_ShouldHaveError_WhenEmpty()
        {
            // Arrange
            var query = new GetHistoricalRatesQuery { StartDate = default };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.StartDate)
                 .WithErrorMessage("Start date is required");
        }

        [Fact]
        public void StartDate_ShouldHaveError_WhenInFuture()
        {
            // Arrange
            var query = new GetHistoricalRatesQuery { StartDate = DateTime.UtcNow.AddDays(1) };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.StartDate)
                 .WithErrorMessage("Start date must be a valid date");
        }

        [Fact]
        public void EndDate_ShouldHaveError_WhenEmpty()
        {
            // Arrange
            var query = new GetHistoricalRatesQuery { EndDate = default };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.EndDate)
                 .WithErrorMessage("End date is required");
        }

        [Fact]
        public void EndDate_ShouldHaveError_WhenInFuture()
        {
            // Arrange
            var query = new GetHistoricalRatesQuery { EndDate = DateTime.UtcNow.AddDays(1) };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.EndDate)
                 .WithErrorMessage("End date must be a valid date");
        }

        [Fact]
        public void EndDate_ShouldHaveError_WhenBeforeStartDate()
        {
            // Arrange
            var query = new GetHistoricalRatesQuery
            {
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(-2)
            };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.EndDate)
                 .WithErrorMessage("End date must be greater than or equal to start date");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Page_ShouldHaveError_WhenNotGreaterThanZero(int page)
        {
            // Arrange
            var query = new GetHistoricalRatesQuery { Page = page };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Page)
                 .WithErrorMessage("Page must be greater than 0");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(101)]
        public void PageSize_ShouldHaveError_WhenOutsideAllowedRange(int pageSize)
        {
            // Arrange
            var query = new GetHistoricalRatesQuery { PageSize = pageSize };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.PageSize)
                 .WithErrorMessage("Page size must be between 1 and 100");
        }

        [Fact]
        public void ShouldHaveError_WhenDateRangeExceeds365Days()
        {
            // Arrange
            var query = new GetHistoricalRatesQuery
            {
                StartDate = DateTime.UtcNow.AddDays(-366),
                EndDate = DateTime.UtcNow
            };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x)
                 .WithErrorMessage("Date range cannot exceed 365 days");
        }

        [Fact]
        public void ShouldNotHaveError_WhenRequestIsValid()
        {
            // Arrange
            var query = new GetHistoricalRatesQuery
            {
                BaseCurrency = "USD",
                StartDate = DateTime.UtcNow.AddDays(-5),
                EndDate = DateTime.UtcNow,
                Page = 1,
                PageSize = 10
            };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
} 