using Microsoft.VisualStudio.TestTools.UnitTesting;
using BambooCardCurrencyConverter.Controllers;
using CurrencyConverterModels.ViewModels;
using System.Threading.Tasks;
using BambooCardCurrencyConverter.Services.CurrencyConverterService;
using Moq;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using CurrencyConverterModels.Helpers;
using Microsoft.Extensions.Caching.Memory;
using System.Dynamic;
using Microsoft.Extensions.DependencyInjection;
using static System.Net.Mime.MediaTypeNames;

namespace BambooCardCurrencyConverterTests
{
    [TestClass]
    public class ControllerTests
    {
        /// <summary>
        /// Tests GetAllCurrencies in CurrencyConverterController.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetAllCurrenciesTest()
        {
            dynamic mockServiceResult = new ExpandoObject();
            mockServiceResult.AUD = "Australian Dollar";
            mockServiceResult.USD = "United States Dollar";
            mockServiceResult.CAD = "Canadian Dollar";
            var wrap = (object)mockServiceResult;

            //Set up the dependency injection
            var mockService = new Mock<ICurrencyConverter>();
            mockService.Setup(service => service.GetAllCurrencies())
                       .ReturnsAsync(wrap);

            var mockCache = GetSystemUnderTest();
            
            //set up controller
            var controller = new CurrencyConverterController(mockService.Object, mockCache);
            //call controller method
            var result = await controller.GetAllCurrencyInfo();

            // Assert
            Assert.IsNotNull(result);
        }
        /// <summary>
        /// Tests GetExchangeRates in CurrencyConverterController.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetExchangeRatesTest()
        {
            dynamic mockServiceResult = new ExpandoObject();
            mockServiceResult.Amount = 1;
            mockServiceResult.Base = "EUR";
            mockServiceResult.Date = new DateTime(2024, 05, 15);
            mockServiceResult.Rates = new Dictionary<string, double> {
                               {"AUD", 1.5055},
                               {"BGN", 3.5067},
                               {"BRL", 5.5023}
                           };
            var wrap = (object)mockServiceResult;

            //Set up the dependency injection
            var mockService = new Mock<ICurrencyConverter>();
            mockService.Setup(service => service.GetExchangeRates("EUR"))
                       .ReturnsAsync(wrap);

            var mockCache = GetSystemUnderTest();

            //set up controller
            var controller = new CurrencyConverterController(mockService.Object, mockCache);
            //call controller method
            var result = await controller.GetExchangeRates();

            // Assert
            Assert.IsNotNull(result);
        }
        /// <summary>
        /// Tests Convert in CurrencyConverterController.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task ConvertCurrencyTest()
        {
            CurrencyConversionFilter currencyConversionFilter = new CurrencyConversionFilter
            {
                Amount = 108.32,
                BaseCurrency = "EUR",
                FinalCurrency = "USD"
            };
            CurrencyConversionResult mockResult = new CurrencyConversionResult
            {
                Amount = 104,
                Date = DateTime.Now,
                _base = "EUR",
                Rates = new Dictionary<string, string>
                {
                    {"USD","108.32" }
                }
            };
            //Set up the dependency injection
            var mockService = new Mock<ICurrencyConverter>();
            mockService.Setup(service => service.Convert(currencyConversionFilter))
                       .ReturnsAsync(mockResult);

            var mockCache = GetSystemUnderTest();

            //set up controller
            var controller = new CurrencyConverterController(mockService.Object, mockCache);
            //call controller method
            var result = await controller.ConvertCurrency(currencyConversionFilter);

            // Assert
            Assert.IsNotNull(result);
        }
        /// <summary>
        /// Tests GetHistory in CurrencyConverterController.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task GetCurrencyHistoryTest()
        {
            CurrencyHistoryFilter currencyHistoryFilter = new CurrencyHistoryFilter
            {
                BaseCurrency = "EUR",
                FinalCurrency = "USD",
                From = new DateTime(2024, 05, 13),
                Take = 10
            };
            CurrencyHistoryResult mockResult = new CurrencyHistoryResult
            {
                Amount = 104,
                _base = "EUR",
                Rates = new Dictionary<string, Dictionary<string, string>>
                {
                    {"2024-05-14",new Dictionary<string, string>{{"USD", "1.8032"} } }
                }
            };
            //Set up the dependency injection
            var mockService = new Mock<ICurrencyConverter>();
            mockService.Setup(service => service.GetHistory(currencyHistoryFilter))
                       .ReturnsAsync(mockResult);

            var mockCache = GetSystemUnderTest();

            //set up controller
            var controller = new CurrencyConverterController(mockService.Object, mockCache);
            //call controller method
            var result = await controller.GetHistory(currencyHistoryFilter);

            // Assert
            Assert.IsNotNull(result);
        }

        public IMemoryCache GetSystemUnderTest()
        {
            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProvider = services.BuildServiceProvider();

            var memoryCache = serviceProvider.GetService<IMemoryCache>();
            return memoryCache;
        }
    }
}