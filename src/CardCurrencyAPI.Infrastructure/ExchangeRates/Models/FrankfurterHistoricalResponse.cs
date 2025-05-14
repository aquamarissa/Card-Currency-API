using System.Text.Json.Serialization;

namespace CardCurrencyAPI.Infrastructure.ExchangeRates.Models
{
    public class FrankfurterHistoricalResponse
    {
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; } = 1;
        
        [JsonPropertyName("base")]
        public string Base { get; set; } = string.Empty;
        
        [JsonPropertyName("start_date")]
        public DateTime StartDate { get; set; }
        
        [JsonPropertyName("end_date")]
        public DateTime EndDate { get; set; }
        
        [JsonPropertyName("rates")]
        public Dictionary<DateTime, Dictionary<string, decimal>> Rates { get; set; } = 
            new Dictionary<DateTime, Dictionary<string, decimal>>();
    }
} 