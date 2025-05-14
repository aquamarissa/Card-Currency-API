namespace CardCurrencyAPI.Domain.Interfaces
{
    public interface ICurrencyExchangeProviderFactory
    {
        /// <summary>
        /// Creates a currency exchange provider instance based on provider name
        /// </summary>
        /// <param name="providerName">Name of the provider to create (e.g., "Frankfurter")</param>
        /// <returns>An implementation of ICurrencyExchangeProvider</returns>
        ICurrencyExchangeProvider CreateProvider(string providerName);
        
        /// <summary>
        /// Creates the default currency exchange provider
        /// </summary>
        /// <returns>The default implementation of ICurrencyExchangeProvider</returns>
        ICurrencyExchangeProvider CreateDefaultProvider();
    }
} 