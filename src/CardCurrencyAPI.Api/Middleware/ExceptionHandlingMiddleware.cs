using System.Net;
using System.Text.Json;
using CardCurrencyAPI.Application.Common.Exceptions;
using CardCurrencyAPI.Domain.Exceptions;
using ValidationException = FluentValidation.ValidationException;

namespace CardCurrencyAPI.Api.Middleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = exception switch
            {
                ValidationException => HttpStatusCode.BadRequest,
                CurrencyNotSupportedException => HttpStatusCode.BadRequest,
                NotFoundException => HttpStatusCode.NotFound,
                _ => HttpStatusCode.InternalServerError
            };

            var response = new
            {
                type = exception.GetType().Name,
                title = GetTitle(statusCode),
                status = (int)statusCode,
                detail = exception.Message,
                errors = exception is ValidationException validationException
                    ? validationException.Errors
                    : null
            };

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)statusCode;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }

        private static string GetTitle(HttpStatusCode statusCode) => statusCode switch
        {
            HttpStatusCode.BadRequest => "Bad Request",
            HttpStatusCode.NotFound => "Not Found",
            HttpStatusCode.InternalServerError => "Internal Server Error",
            _ => "An error occurred"
        };
    }
} 