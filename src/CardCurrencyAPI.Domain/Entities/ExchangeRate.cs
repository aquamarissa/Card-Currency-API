namespace CardCurrencyAPI.Domain.Entities
{
    public class ExchangeRate
    {
        public string Base { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public Dictionary<string, decimal> Rates { get; set; } = new Dictionary<string, decimal>();
    }
} 