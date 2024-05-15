namespace BambooCardCurrencyConverter.Configurations
{
    using CurrencyConverterModels.CustomException;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;

    public class GlobalExceptionFilter : IAsyncExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case DataNotFoundException:
                    context.Result = new ObjectResult(context.Exception.Message)
                    {
                        StatusCode = 404
                    };
                    break;
                case BadRequestException:
                    context.Result = new ObjectResult(context.Exception.Message)
                    {
                        StatusCode = 400
                    };
                    break;
                default:
                    _logger.LogError(context.Exception, "An unhandled exception occurred.");

                    context.Result = new ObjectResult("An error occurred")
                    {
                        StatusCode = 500
                    };
                    break;
            }

            context.ExceptionHandled = true;
        }
    }

}
