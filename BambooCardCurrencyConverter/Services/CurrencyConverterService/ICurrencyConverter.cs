using CurrencyConverterModels.ViewModels;

namespace BambooCardCurrencyConverter.Services.CurrencyConverterService
{
    public interface ICurrencyConverter
    {
        Task<dynamic> GetAllCurrencies();
        Task<dynamic> GetExchangeRates(string baseCurrency);
        Task<CurrencyConversionResult> Convert(CurrencyConversionFilter currencyConversionFilter);
        Task<CurrencyHistoryResult> GetHistory(CurrencyHistoryFilter currencyHistoryFilter);
    }
}
