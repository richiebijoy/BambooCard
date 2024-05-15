using BambooCardCurrencyConverter.Services.CurrencyConverterService;
using CurrencyConverterModels.ViewModels;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace BambooCardCurrencyConverter.Configurations
{
    public static class RegisterDpenendentServices
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging(builder =>
            {
                builder.AddConsole();
            });

            services.AddControllers(o =>
            {
                //can add Filters as required here
                o.Filters.Add<GlobalExceptionFilter>();
            });
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<CurrencyConversionFilterValidator>();
            services.AddValidatorsFromAssemblyContaining<CurrencyHistoryFilterValidator>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(swaggerConfig =>
            {
                swaggerConfig.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                swaggerConfig.IncludeXmlComments(xmlPath);

                var referencedAssemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
                foreach (var assemblyName in referencedAssemblies)
                {
                    var referencedAssembly = Assembly.Load(assemblyName);
                    var referencedXmlFile = $"{referencedAssembly.GetName().Name}.xml";
                    var referencedXmlPath = Path.Combine(AppContext.BaseDirectory, referencedXmlFile);
                    if (File.Exists(referencedXmlPath))
                    {
                        swaggerConfig.IncludeXmlComments(referencedXmlPath);
                    }
                }
            });

            services.Configure<CurrencyAPIConfiguration>(configuration.GetSection(nameof(CurrencyAPIConfiguration)));

            return services;
        }

        public static IServiceCollection RegisterDependencies(this IServiceCollection services)
        {
            services.AddScoped<ICurrencyConverter, CurrencyConverter>();
            return services;
        }
    }
}



