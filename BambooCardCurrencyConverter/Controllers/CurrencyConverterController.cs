using BambooCardCurrencyConverter.Services.CurrencyConverterService;
using CurrencyConverterModels.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public CurrencyConverterController(ICurrencyConverter currencyConverter)
        {
            _currencyConverter = currencyConverter;
        }
        /// <summary>
        /// This is an API to get all currency symbols and their respective full forms so that users know which currencies are supported by the app.
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllCurrencyInfo")]
        public async Task<ActionResult<dynamic>> GetAllCurrencyInfo()
        {
            return Ok(await _currencyConverter.GetAllCurrencies());
        }
        /// <summary>
        /// This is the First Endpoint. It is used to get the current exchange rates relative to a base currency.
        /// </summary>
        /// <param name="baseCurrency">"EUR"</param>
        /// <returns></returns>
        [HttpGet("GetExchangeRates")]
        public async Task<ActionResult<dynamic>> GetExchangeRates(string baseCurrency = "EUR")
        {
            return Ok(await _currencyConverter.GetExchangeRates(baseCurrency));
        }
        /// <summary>
        /// This is the Second Endpoint. It is used for currency conversion.
        /// </summary>
        /// <param name="currencyConversionFilter"></param>
        /// <returns></returns>
        [HttpPost("ConvertCurrency")]
        public async Task<ActionResult<CurrencyConversionResult>> ConvertCurrency(CurrencyConversionFilter currencyConversionFilter)
        {
            return Ok(await _currencyConverter.Convert(currencyConversionFilter));
        }
        /// <summary>
        /// This is the Third Endpoint. It is used to get historical currency rates.
        /// </summary>
        /// <param name="currencyHistoryFilter"></param>
        /// <returns></returns>
        [HttpPost("GetHistory")]
        public async Task<ActionResult<CurrencyHistoryResult>> GetHistory(CurrencyHistoryFilter currencyHistoryFilter)
        {
            return Ok(await _currencyConverter.GetHistory(currencyHistoryFilter));
        }
    }
}
