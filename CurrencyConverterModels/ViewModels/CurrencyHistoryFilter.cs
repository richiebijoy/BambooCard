using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverterModels.ViewModels
{
    public class CurrencyHistoryFilter
    {
        /// <summary>
        /// Default is "USD".
        /// </summary>
        /// 
        public string BaseCurrency { get; set; }
        /// <summary>
        /// Default is "USD".
        /// </summary>
        /// 
        public string FinalCurrency { get; set; }
        /// <summary>
        /// Default is "2024-05-16"
        /// </summary>
        public DateTime From { get; set; }
        public DateTime? To { get; set; }
        /// <summary>
        /// Default is 10.
        /// </summary>
        public int Take { get; set; }
        /// <summary>
        /// Default is 10.
        /// </summary>
        public int Skip { get; set; } = 10;
        public CurrencyHistoryFilter()
        {
            FinalCurrency = "USD";
            Take = 10;
            Skip = 0;
        }
    }
    public class CurrencyHistoryFilterValidator : AbstractValidator<CurrencyHistoryFilter>
    {
        public CurrencyHistoryFilterValidator()
        {
            RuleFor(x => x.FinalCurrency).NotEmpty();
            RuleFor(x => x.From).GreaterThanOrEqualTo(new DateTime(1999,1,4));
            RuleFor(x => x.Take).NotEmpty();
        }
    }
}
