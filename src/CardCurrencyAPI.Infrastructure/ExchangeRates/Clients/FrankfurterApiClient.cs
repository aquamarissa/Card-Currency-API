using System.Text.Json;
using CardCurrencyAPI.Domain.Exceptions;
using CardCurrencyAPI.Infrastructure.ExchangeRates.Configuration;
using CardCurrencyAPI.Infrastructure.ExchangeRates.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CardCurrencyAPI.Infrastructure.ExchangeRates.Clients
{
    public class FrankfurterApiClient : IFrankfurterApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FrankfurterApiClient> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public FrankfurterApiClient(
            HttpClient httpClient,
            IOptions<FrankfurterApiOptions> options,
            ILogger<FrankfurterApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            
            _httpClient.BaseAddress = new Uri(options.Value.BaseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(options.Value.TimeoutSeconds);
            
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<FrankfurterLatestResponse> GetLatestRatesAsync(string baseCurrency, CancellationToken cancellationToken = default)
        {
            var endpoint = $"/latest?base={baseCurrency}";
            return await SendRequestAsync<FrankfurterLatestResponse>(endpoint, cancellationToken);
        }

        public async Task<FrankfurterHistoricalResponse> GetHistoricalRatesAsync(
            string baseCurrency, 
            DateTime startDate, 
            DateTime endDate, 
            CancellationToken cancellationToken = default)
        {
            var endpoint = $"/{startDate:yyyy-MM-dd}..{endDate:yyyy-MM-dd}?base={baseCurrency}";
            return await SendRequestAsync<FrankfurterHistoricalResponse>(endpoint, cancellationToken);
        }

        public async Task<FrankfurterCurrenciesResponse> GetCurrenciesAsync(CancellationToken cancellationToken = default)
        {
            var endpoint = "/currencies";
            return await SendRequestAsync<FrankfurterCurrenciesResponse>(endpoint, cancellationToken);
        }

        private async Task<T> SendRequestAsync<T>(string endpoint, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Sending request to Frankfurter API: {Endpoint}", endpoint);
                
                var response = await _httpClient.GetAsync(endpoint, cancellationToken);
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var result = JsonSerializer.Deserialize<T>(content, _jsonSerializerOptions);
                
                if (result == null)
                {
                    throw new ExternalApiException("Frankfurter", endpoint, "Failed to deserialize response");
                }
                
                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error occurred while calling Frankfurter API: {Endpoint}", endpoint);
                throw new ExternalApiException("Frankfurter", endpoint, "HTTP request failed", ex);
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                _logger.LogError(ex, "Request to Frankfurter API timed out: {Endpoint}", endpoint);
                throw new ExternalApiException("Frankfurter", endpoint, "Request timed out", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize response from Frankfurter API: {Endpoint}", endpoint);
                throw new ExternalApiException("Frankfurter", endpoint, "Failed to deserialize response", ex);
            }
            catch (Exception ex) when (ex is not ExternalApiException)
            {
                _logger.LogError(ex, "Unexpected error occurred while calling Frankfurter API: {Endpoint}", endpoint);
                throw new ExternalApiException("Frankfurter", endpoint, "Unexpected error", ex);
            }
        }
    }
} 