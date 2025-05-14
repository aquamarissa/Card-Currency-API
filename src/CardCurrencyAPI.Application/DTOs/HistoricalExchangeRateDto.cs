namespace CardCurrencyAPI.Application.DTOs
{
    public class HistoricalExchangeRateDto
    {
        public string Base { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Dictionary<DateTime, Dictionary<string, decimal>> Rates { get; set; } = new Dictionary<DateTime, Dictionary<string, decimal>>();
        
        public PaginationMetadata Pagination { get; set; } = new PaginationMetadata();
    }
    
    public class PaginationMetadata
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
    }
} 