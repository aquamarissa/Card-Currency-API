namespace CardCurrencyAPI.Domain.Entities
{
    public class HistoricalExchangeRate
    {
        public string Base { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Dictionary<DateTime, Dictionary<string, decimal>> Rates { get; set; } = new Dictionary<DateTime, Dictionary<string, decimal>>();
        
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
    }
} 