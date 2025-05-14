using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CardCurrencyAPI.Application.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
            : IPipelineBehavior<TRequest, TResponse>
            where TRequest : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var requestId = Guid.NewGuid().ToString();
            
            logger.LogInformation("[START] {RequestName} {RequestId}", requestName, requestId);
            
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var response = await next();
                stopwatch.Stop();
                
                logger.LogInformation("[END] {RequestName} {RequestId} completed in {ElapsedMilliseconds}ms", 
                    requestName, requestId, stopwatch.ElapsedMilliseconds);
                
                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                
                logger.LogError(ex, "[ERROR] {RequestName} {RequestId} failed after {ElapsedMilliseconds}ms", 
                    requestName, requestId, stopwatch.ElapsedMilliseconds);
                
                throw;
            }
        }
    }
} 