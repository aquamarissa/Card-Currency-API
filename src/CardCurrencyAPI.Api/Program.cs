using CardCurrencyAPI.Api.Endpoints;
using CardCurrencyAPI.Api.Middleware;
using CardCurrencyAPI.Application;
using CardCurrencyAPI.Infrastructure;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Threading.RateLimiting;
using CardCurrencyAPI.Api.Endpoints.ExchangeRates;

namespace CardCurrencyAPI.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configure Logger
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            try
            {
                Log.Information("Starting web application");
                
                var builder = WebApplication.CreateBuilder(args);

                // Configure Serilog
                builder.Host.UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext());

                // Register services
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "CardCurrency API",
                        Version = "v1",
                        Description = "API for currency exchange rates and conversions",
                        Contact = new OpenApiContact
                        {
                            Name = "API Support",
                            Email = "support@cardcurrency.com"
                        }
                    });
                });

                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowFrontend", policy =>
                    {
                        policy.WithOrigins("http://localhost:3000")
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
                });

                builder.Services.AddRateLimiter(options =>
                {
                    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                        RateLimitPartition.GetFixedWindowLimiter("GlobalLimit",
                            _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = 100,
                                Window = TimeSpan.FromMinutes(1)
                            }));
                });

                builder.Services.AddHealthChecks();
                builder.Services.AddApplication();
                builder.Services.AddInfrastructure(builder.Configuration);

                var app = builder.Build();

                // Configure middleware
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CardCurrency API v1");
                        c.RoutePrefix = string.Empty;
                    });
                }

                app.UseSerilogRequestLogging();
                app.UseMiddleware<RequestLoggingMiddleware>();
                app.UseMiddleware<ExceptionHandlingMiddleware>();
                app.UseCors("AllowFrontend");
                app.UseRateLimiter();

                // Map all API endpoints from separate classes
                app.MapExchangeRatesEndpoints();
                app.MapHealthCheckEndpoints();

                // Start application
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
} 