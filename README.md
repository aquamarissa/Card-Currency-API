# Card Currency API

A .NET API for currency exchange rates and conversion, utilizing the Frankfurter API as the data source.

## Project Overview

CardCurrencyAPI is a RESTful web API built with .NET 7 that provides currency exchange functionality with the following features:

- Currency conversion between supported currencies
- Retrieval of latest exchange rates
- Historical exchange rate data with pagination
- Robust error handling and request logging
- Rate limiting to prevent abuse
- Health checks for monitoring
- Swagger API documentation

## Architecture

The application follows Clean Architecture principles, divided into several layers:

### Domain Layer
Contains the core business entities and interfaces:
- `ExchangeRate` and `HistoricalExchangeRate` entities
- `ICurrencyExchangeProvider` interface defining core operations

### Application Layer
Contains application logic, CQRS implementation with MediatR:
- Queries and Commands for currency operations
- Validators using FluentValidation
- Response models

### Infrastructure Layer
Implements external service interactions:
- `FrankfurterExchangeProvider` for the Frankfurter API integration
- HTTP clients with Polly for resilience
- Caching mechanism

### API Layer
Handles HTTP requests and responses:
- Minimal API endpoints
- Middleware for exception handling and request logging
- Swagger documentation

## API Endpoints

### Currency Exchange
- `GET /api/v1/exchange-rates/latest` - Get latest exchange rates
- `GET /api/v1/exchange-rates/historical` - Get historical exchange rates
- `POST /api/v1/exchange-rates/convert` - Convert between currencies

### API Examples

#### Get Latest Exchange Rates
Request:
```
GET /api/v1/exchange-rates/latest?BaseCurrency=USD
```

Response:
```json
{
  "base": "USD",
  "date": "2023-06-14",
  "rates": {
    "EUR": 0.9248,
    "GBP": 0.7889,
    "JPY": 140.38,
    "CAD": 1.3342
  }
}
```

#### Convert Currency
Request:
```
POST /api/v1/exchange-rates/convert
Content-Type: application/json

{
  "amount": 100,
  "fromCurrency": "USD",
  "toCurrency": "EUR"
}
```

Response:
```json
{
  "amount": 100.00,
  "fromCurrency": "USD",
  "toCurrency": "EUR",
  "result": 92.48,
  "rate": 0.9248,
  "date": "2023-06-14"
}
```

#### Get Historical Rates
Request:
```
GET /api/v1/exchange-rates/historical?BaseCurrency=EUR&StartDate=2023-01-01&EndDate=2023-01-10&Page=1&PageSize=5
```

Response:
```json
{
  "base": "EUR",
  "startDate": "2023-01-01",
  "endDate": "2023-01-10",
  "rates": {
    "2023-01-02": {
      "USD": 1.0812,
      "GBP": 0.8822,
      "JPY": 142.58
    },
    "2023-01-03": {
      "USD": 1.0743,
      "GBP": 0.8836,
      "JPY": 141.97
    }
  },
  "currentPage": 1,
  "totalPages": 2,
  "pageSize": 5,
  "totalItems": 10
}
```

### Health Checks
- `GET /health` - Check API health status
- `GET /health/ready` - Check API readiness
- `GET /health/live` - Check API liveness

## Error Handling

The API uses standardized error responses with appropriate HTTP status codes:

### Validation Errors (400 Bad Request)
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-1234abcd56789-abcdef012345-00",
  "errors": {
    "Amount": ["Amount must be greater than zero."],
    "FromCurrency": ["Currency TRY is not supported."]
  }
}
```

### Not Found Errors (404 Not Found)
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "The specified resource was not found.",
  "status": 404,
  "detail": "Exchange rate not found for date 2022-01-01",
  "traceId": "00-1234abcd56789-abcdef012345-00"
}
```

### Server Errors (500 Internal Server Error)
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",
  "title": "An error occurred while processing your request.",
  "status": 500,
  "traceId": "00-1234abcd56789-abcdef012345-00"
}
```

## Configuration

The API is configured using appsettings.json with the following sections:

```json
"ExchangeRates": {
  "FrankfurterApi": {
    "BaseUrl": "https://api.frankfurter.app",
    "CacheExpirationMinutes": 30,
    "RetryCount": 3,
    "RetryDelayMilliseconds": 500,
    "TimeoutSeconds": 30,
    "CircuitBreakerFailuresBeforeBreaking": 3
  }
}
```

## Resilience Features

- **Circuit Breaker Pattern**: Prevents cascading failures when the external API is unavailable
- **Retry Mechanism**: Automatically retries failed requests with exponential backoff
- **Caching**: Reduces load on the external API and improves response times
- **Request Timeout**: Prevents hanging requests
- **Input Validation**: Ensures all requests are valid before processing

## Setup and Running

### Prerequisites
- .NET 7 SDK
- An IDE like Visual Studio, VS Code, or JetBrains Rider

### Running the API
1. Clone the repository
2. Navigate to the project directory
3. Run the following commands:

```bash
cd src/CardCurrencyAPI.Api
dotnet run
```

The API will be available at http://localhost:5000 with Swagger UI at http://localhost:5000/swagger.

## Testing

The project includes comprehensive tests:
- Unit tests for all components
- Integration tests for API endpoints
- Mocked external dependencies for consistent testing

Run tests with:
```bash
dotnet test
```

## Limitations

The following currencies are currently not supported:
- TRY (Turkish Lira)
- PLN (Polish Złoty)
- THB (Thai Baht)
- MXN (Mexican Peso)

## Future Improvements

Potential enhancements for future versions:

1. **Authentication and Authorization**
   - Implement JWT authentication
   - Role-based access control for certain endpoints
   - API key rate limiting

2. **Additional Exchange Rate Providers**
   - Integration with additional providers (European Central Bank, Open Exchange Rates)
   - Implement failover between providers
   - Provider comparison features

3. **Enhanced Features**
   - Currency conversion charts and trends
   - Email notifications for significant rate changes
   - Batch conversion operations

4. **Infrastructure Improvements**
   - Docker containerization
   - Kubernetes deployment configuration
   - CI/CD pipeline with GitHub Actions
   - Distributed caching with Redis 