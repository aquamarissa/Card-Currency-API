namespace CardCurrencyAPI.Domain.Exceptions
{
    public class CurrencyNotSupportedException(string currency)
            : Exception($"Currency '{currency}' is not supported. TRY, PLN, THB, and MXN are excluded currencies.")
    {
        public string Currency { get; } = currency;
    }
} 