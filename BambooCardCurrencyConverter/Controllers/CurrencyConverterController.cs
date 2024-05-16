using BambooCardCurrencyConverter.Services.CurrencyConverterService;
using CurrencyConverterModels.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Dynamic;

namespace BambooCardCurrencyConverter.Controllers
{
    /// <summary>
    /// This is the controller for API's related to Franfurt's API's.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyConverterController : ControllerBase
    {
        private readonly ICurrencyConverter _currencyConverter;
        private IMemoryCache _cache;
        public CurrencyConverterController(ICurrencyConverter currencyConverter, IMemoryCache cache)
        {
            _currencyConverter = currencyConverter;
            _cache = cache;
        }
        /// <summary>
        /// This is an API to get all currency symbols and their respective full forms so that users know which currencies are supported by the app.
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllCurrencyInfo")]
        public async Task<ActionResult<dynamic>> GetAllCurrencyInfo()
        {
            string cacheKey = $"GetAllCurrencyInfo_{DateTime.Now.ToShortDateString()}";
            // Check if data exists in cache
            if (!_cache.TryGetValue(cacheKey, out ExpandoObject? data))
            {
                data = await _currencyConverter.GetAllCurrencies();
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24f)
                };
                _cache.Set(cacheKey, data, cacheOptions);
            }
            return Ok(data);
        }
        /// <summary>
        /// This is the First Endpoint. It is used to get the current exchange rates relative to a base currency.
        /// </summary>
        /// <param name="baseCurrency">"EUR"</param>
        /// <returns></returns>
        [HttpGet("GetExchangeRates")]
        public async Task<ActionResult<dynamic>> GetExchangeRates(string baseCurrency = "EUR")
        {
            string cacheKey = $"GetExchangeRates_{baseCurrency}";
            if (!_cache.TryGetValue(cacheKey, out ExpandoObject? data))
            {
                data = await _currencyConverter.GetExchangeRates(baseCurrency);
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    //AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5f)
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30f)
                };
                _cache.Set(cacheKey, data, cacheOptions);
            }
            return Ok(data);
        }
        /// <summary>
        /// This is the Second Endpoint. It is used for currency conversion.
        /// </summary>
        /// <param name="currencyConversionFilter"></param>
        /// <returns></returns>
        [HttpPost("ConvertCurrency")]
        public async Task<ActionResult<CurrencyConversionResult>> ConvertCurrency(CurrencyConversionFilter currencyConversionFilter)
        {
            string cacheKey = $"ConvertCurrency_{currencyConversionFilter.BaseCurrency}_{currencyConversionFilter.FinalCurrency}_{currencyConversionFilter.Amount}";
            if (!_cache.TryGetValue(cacheKey, out CurrencyConversionResult? data))
            {
                data = await _currencyConverter.Convert(currencyConversionFilter);
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    //AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5f)
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30f)
                };
                _cache.Set(cacheKey, data, cacheOptions);
            }
            return Ok(data);
        }
        /// <summary>
        /// This is the Third Endpoint. It is used to get historical currency rates.
        /// </summary>
        /// <param name="currencyHistoryFilter"></param>
        /// <returns></returns>
        [HttpPost("GetHistory")]
        public async Task<ActionResult<CurrencyHistoryResult>> GetHistory(CurrencyHistoryFilter currencyHistoryFilter)
        {
            //can create a special hash using currencyHistoryFilter values and store as cacheKey.
            return Ok(await _currencyConverter.GetHistory(currencyHistoryFilter));
        }
    }
}
