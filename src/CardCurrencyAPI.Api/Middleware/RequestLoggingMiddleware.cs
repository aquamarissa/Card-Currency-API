using System.Diagnostics;

namespace CardCurrencyAPI.Api.Middleware
{
    public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestId = Guid.NewGuid().ToString();

            try
            {
                using (logger.BeginScope(new { RequestId = requestId }))
                {
                    logger.LogInformation(
                        "Request {RequestMethod} {RequestPath} started",
                        context.Request.Method,
                        context.Request.Path);

                    await next(context);

                    stopwatch.Stop();

                    logger.LogInformation(
                        "Request {RequestMethod} {RequestPath} completed with status code {StatusCode} in {ElapsedMilliseconds} ms",
                        context.Request.Method,
                        context.Request.Path,
                        context.Response.StatusCode,
                        stopwatch.ElapsedMilliseconds);
                }
            }
            catch (Exception)
            {
                stopwatch.Stop();

                logger.LogError(
                    "Request {RequestMethod} {RequestPath} failed after {ElapsedMilliseconds} ms",
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.ElapsedMilliseconds);

                throw;
            }
        }
    }
} 