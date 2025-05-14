using CardCurrencyAPI.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace CardCurrencyAPI.Tests.Middleware
{
    public class RequestLoggingMiddlewareTests
    {
        private readonly Mock<ILogger<RequestLoggingMiddleware>> _loggerMock = new();
        private readonly HttpContext _context = new DefaultHttpContext();
        private readonly RequestDelegate _nextMock = _ => Task.CompletedTask;

        [Fact]
        public async Task InvokeAsync_LogsRequestStart()
        {
            // Arrange
            _context.Request.Method = "GET";
            _context.Request.Path = "/api/test";

            var middleware = new RequestLoggingMiddleware(_nextMock, _loggerMock.Object);

            // Act
            await middleware.InvokeAsync(_context);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Request GET /api/test started")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_LogsRequestCompletion()
        {
            // Arrange
            _context.Request.Method = "POST";
            _context.Request.Path = "/api/test";
            _context.Response.StatusCode = 200;

            var middleware = new RequestLoggingMiddleware(_nextMock, _loggerMock.Object);

            // Act
            await middleware.InvokeAsync(_context);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Request POST /api/test completed with status code 200")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_LogsError_WhenExceptionOccurs()
        {
            // Arrange
            _context.Request.Method = "GET";
            _context.Request.Path = "/api/test";

            var exception = new Exception("Test exception");
            var middleware = new RequestLoggingMiddleware(
                _ => throw exception,
                _loggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => middleware.InvokeAsync(_context));

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Request GET /api/test failed")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_IncludesRequestId_InLogScope()
        {
            // Arrange
            var middleware = new RequestLoggingMiddleware(_nextMock, _loggerMock.Object);

            // Act
            await middleware.InvokeAsync(_context);

            // Assert
            _loggerMock.Verify(
                x => x.BeginScope(
                    It.Is<object>(v => v.ToString().Contains("RequestId"))),
                Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_MeasuresExecutionTime()
        {
            // Arrange
            var delay = TimeSpan.FromMilliseconds(100);
            var nextMock = new RequestDelegate(async _ => await Task.Delay(delay));
            var middleware = new RequestLoggingMiddleware(nextMock, _loggerMock.Object);

            // Act
            await middleware.InvokeAsync(_context);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<object>(v => v.ToString().Contains("ms")),
                    null,
                It.IsAny<Func<object, Exception, string>>()),
                Times.AtLeastOnce);
        }
    }
} 