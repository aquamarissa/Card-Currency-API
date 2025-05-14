using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;

namespace CardCurrencyAPI.Api.Endpoints;

public static class HealthCheckEndpoints
{
    public static void MapHealthCheckEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                
                var response = new
                {
                    Status = report.Status.ToString(),
                    Duration = report.TotalDuration,
                    Checks = report.Entries.Select(e => new
                    {
                        Name = e.Key,
                        Status = e.Value.Status.ToString(),
                        Duration = e.Value.Duration,
                        Description = e.Value.Description
                    })
                };
                
                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(response, new JsonSerializerOptions 
                    { 
                        WriteIndented = true 
                    }));
            }
        })
        .WithTags("Health")
        .WithName("HealthCheck")
        .WithDescription("Returns the health status of the API and its dependencies")
        .WithOpenApi();
    }
} 