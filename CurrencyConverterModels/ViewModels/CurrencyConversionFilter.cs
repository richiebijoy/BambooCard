using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyConverterModels.Helpers;
using FluentValidation;

namespace CurrencyConverterModels.ViewModels
{
    public class CurrencyConversionFilter
    {
        /// <summary>
        /// Default is "EUR".
        /// </summary>
        public string BaseCurrency { get; set; }
        public string FinalCurrency { get; set; }
        public double Amount { get; set; }
    }
    public class CurrencyConversionFilterValidator : AbstractValidator<CurrencyConversionFilter>
    {
        public CurrencyConversionFilterValidator()
        {
            RuleFor(x => x.Amount).NotEqual(0).WithMessage("Amount cannot be 0.");
            //RuleFor(x => x.FinalCurrency).NotEmpty().WithMessage("Final Currency cannot be empty.");
            RuleFor(x => x.BaseCurrency).NotEmpty().WithMessage("Base Currency cannot be empty.");
        }
    }
}
