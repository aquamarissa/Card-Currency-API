namespace CardCurrencyAPI.Domain.Exceptions
{
    public class ExternalApiException : Exception
    {
        public string ApiName { get; }
        public string Endpoint { get; }

        public ExternalApiException(string apiName, string endpoint, string message) 
            : base($"Error calling {apiName} API at endpoint '{endpoint}': {message}")
        {
            ApiName = apiName;
            Endpoint = endpoint;
        }

        public ExternalApiException(string apiName, string endpoint, string message, Exception innerException) 
            : base($"Error calling {apiName} API at endpoint '{endpoint}': {message}", innerException)
        {
            ApiName = apiName;
            Endpoint = endpoint;
        }
    }
} 