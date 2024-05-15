using BambooCardCurrencyConverter.Configurations;
using CurrencyConverterModels.CustomException;
using CurrencyConverterModels.Helpers;
using CurrencyConverterModels.ViewModels;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using System.Dynamic;
using System.Linq;

namespace BambooCardCurrencyConverter.Services.CurrencyConverterService
{
    public class CurrencyConverter : ICurrencyConverter
    {
        private readonly HttpClient _httpClient;
        private readonly CurrencyAPIConfiguration _currencyAPIConfiguration;
        private readonly ILogger<CurrencyConverter> _logger;
        public CurrencyConverter(IOptions<CurrencyAPIConfiguration> currencyAPIConfiguration, ILogger<CurrencyConverter> logger)
        {
            _httpClient = new HttpClient();
            _currencyAPIConfiguration = currencyAPIConfiguration.Value;
            _logger = logger;
        }
        public async Task<dynamic> GetAllCurrencies()
        {
            Uri uri = new Uri($"{_currencyAPIConfiguration.CurrencyBaseUri}/currencies");
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync(uri);
            httpResponseMessage.EnsureSuccessStatusCode();
            string response = await httpResponseMessage.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(response))
            {
                throw new DataNotFoundException("Sorry we couldn't find any currencies at the moment. Please try again later.");
            }
            dynamic currencyInfos = JsonConvert.DeserializeObject<ExpandoObject>(response);
            return currencyInfos;
        }
        public async Task<dynamic> GetExchangeRates(string baseCurrency)
        {
            AsyncRetryPolicy retryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(_currencyAPIConfiguration.Retry, retryAttempt => TimeSpan.FromSeconds(10f));

            return await retryPolicy.ExecuteAsync<dynamic>(async () =>
            {
                Uri uri = new Uri($"{_currencyAPIConfiguration.CurrencyBaseUri}/latest?from={baseCurrency}");
                HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync(uri);
                httpResponseMessage.EnsureSuccessStatusCode();
                string response = await httpResponseMessage.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(response))
                {
                    throw new DataNotFoundException("Sorry we couldn't find any currencies at the moment. Please try again later.");
                }
                dynamic result = JsonConvert.DeserializeObject<ExpandoObject>(response);
                return result;
            });
        }
        public async Task<CurrencyConversionResult> Convert(CurrencyConversionFilter currencyConversionFilter)
        {
            if (!currencyConversionFilter.BaseCurrency.ValidateCurrencyExceptions())
            {
                throw new BadRequestException($"Conversion from {currencyConversionFilter.BaseCurrency} is not allowed!");
            }
            AsyncRetryPolicy retryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(_currencyAPIConfiguration.Retry, retryAttempt => TimeSpan.FromSeconds(10f));

            return await retryPolicy.ExecuteAsync(async () =>
            {
                Dictionary<string, string> queryParams = new Dictionary<string, string>
                {
                    {"amount", $"{currencyConversionFilter.Amount}" },
                    {"from", currencyConversionFilter.BaseCurrency }
                };

                if (!string.IsNullOrEmpty(currencyConversionFilter.FinalCurrency))
                {
                    queryParams.Add("to", currencyConversionFilter.FinalCurrency);
                }

                Uri uri = new Uri(QueryHelpers.AddQueryString($"{_currencyAPIConfiguration.CurrencyBaseUri}/latest", queryParams));
                HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync(uri);
                httpResponseMessage.EnsureSuccessStatusCode();
                string response = await httpResponseMessage.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(response))
                {
                    throw new DataNotFoundException("Sorry we couldn't find any currencies at the moment. Please try again later.");
                }
                CurrencyConversionResult result = JsonConvert.DeserializeObject<CurrencyConversionResult>(response);
                result.Rates = result.Rates.Where(x => x.Key.ValidateCurrencyExceptions()).ToDictionary();
                return result;
            });
        }
        public async Task<CurrencyHistoryResult> GetHistory(CurrencyHistoryFilter currencyHistoryFilter)
        {
            AsyncRetryPolicy retryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(_currencyAPIConfiguration.Retry, retryAttempt => TimeSpan.FromSeconds(10f));

            return await retryPolicy.ExecuteAsync<dynamic>(async () =>
            {
                Dictionary<string, string> queryParams = new Dictionary<string, string>
                {
                    {"to", $"{currencyHistoryFilter.FinalCurrency}" }
                };

                if (string.IsNullOrEmpty(currencyHistoryFilter.BaseCurrency))
                {
                    queryParams.Add("from", "EUR");
                }

                string datePath = $"{currencyHistoryFilter.From.GetDateForHistoricalApi()}..";
                if(currencyHistoryFilter.To != null)
                {
                    datePath = $"{datePath}{currencyHistoryFilter.To.Value.GetDateForHistoricalApi()}";
                }
                
                Uri uri = new Uri(QueryHelpers.AddQueryString($"{_currencyAPIConfiguration.CurrencyBaseUri}/{datePath}", queryParams));
                HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync(uri);
                httpResponseMessage.EnsureSuccessStatusCode();
                string response = await httpResponseMessage.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(response))
                {
                    throw new DataNotFoundException("Sorry we couldn't find any currencies at the moment. Please try again later.");
                }
                CurrencyHistoryResult result = JsonConvert.DeserializeObject<CurrencyHistoryResult>(response);
                result.TotalResults = result.Rates.Count;
                result.Rates = result.Rates.Skip(currencyHistoryFilter.Skip).Take(currencyHistoryFilter.Take).ToDictionary();
                result.PageCount = (int)Math.Ceiling((double)currencyHistoryFilter.Skip / currencyHistoryFilter.Take) + 1;
                return result;
            });
        }
    }
}
