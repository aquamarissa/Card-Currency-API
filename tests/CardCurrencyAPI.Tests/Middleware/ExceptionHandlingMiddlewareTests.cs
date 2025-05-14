using System.Net;
using System.Text.Json;
using CardCurrencyAPI.Api.Middleware;
using CardCurrencyAPI.Application.Common.Exceptions;
using CardCurrencyAPI.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using ValidationException = FluentValidation.ValidationException;

namespace CardCurrencyAPI.Tests.Middleware
{
    public class ExceptionHandlingMiddlewareTests
    {
        private readonly Mock<ILogger<ExceptionHandlingMiddleware>> _loggerMock;
        private readonly HttpContext _context;

        public ExceptionHandlingMiddlewareTests()
        {
            _loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
            _context = new DefaultHttpContext();
            _context.Response.Body = new System.IO.MemoryStream();
        }

        [Theory]
        [InlineData(typeof(ValidationException), HttpStatusCode.BadRequest)]
        [InlineData(typeof(CurrencyNotSupportedException), HttpStatusCode.BadRequest)]
        [InlineData(typeof(NotFoundException), HttpStatusCode.NotFound)]
        [InlineData(typeof(Exception), HttpStatusCode.InternalServerError)]
        public async Task InvokeAsync_SetsCorrectStatusCode_ForDifferentExceptions(Type exceptionType, HttpStatusCode expectedStatusCode)
        {
            // Arrange
            var middleware = new ExceptionHandlingMiddleware(
                next: _ => throw ((Exception)Activator.CreateInstance(exceptionType, "Test message")!),
                logger: _loggerMock.Object);

            // Act
            await middleware.InvokeAsync(_context);

            // Assert
            Assert.Equal((int)expectedStatusCode, _context.Response.StatusCode);
            Assert.Equal("application/problem+json", _context.Response.ContentType);
        }

        [Fact]
        public async Task InvokeAsync_IncludesValidationErrors_ForValidationException()
        {
            // Arrange
            var validationException = new ValidationException("Test validation error");
            var middleware = new ExceptionHandlingMiddleware(
                next: _ => throw validationException,
                logger: _loggerMock.Object);

            // Act
            await middleware.InvokeAsync(_context);

            // Assert
            _context.Response.Body.Position = 0;
            using var reader = new System.IO.StreamReader(_context.Response.Body);
            var responseBody = await reader.ReadToEndAsync();
            var response = JsonSerializer.Deserialize<JsonElement>(responseBody);

            Assert.Equal("ValidationException", response.GetProperty("type").GetString());
            Assert.Equal("Bad Request", response.GetProperty("title").GetString());
            Assert.Equal(400, response.GetProperty("status").GetInt32());
            Assert.Equal(validationException.Message, response.GetProperty("detail").GetString());
        }

        [Fact]
        public async Task InvokeAsync_LogsError_WhenExceptionOccurs()
        {
            // Arrange
            var exception = new Exception("Test exception");
            var middleware = new ExceptionHandlingMiddleware(
                next: _ => throw exception,
                logger: _loggerMock.Object);

            // Act
            await middleware.InvokeAsync(_context);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    exception,
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }
    }
} 