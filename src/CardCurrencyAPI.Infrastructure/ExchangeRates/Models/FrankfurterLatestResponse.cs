using System.Text.Json.Serialization;

namespace CardCurrencyAPI.Infrastructure.ExchangeRates.Models
{
    public class FrankfurterLatestResponse
    {
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; } = 1;
        
        [JsonPropertyName("base")]
        public string Base { get; set; } = string.Empty;
        
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }
        
        [JsonPropertyName("rates")]
        public Dictionary<string, decimal> Rates { get; set; } = new Dictionary<string, decimal>();
    }
} 