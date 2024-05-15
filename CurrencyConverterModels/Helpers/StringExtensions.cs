using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverterModels.Helpers
{
    public static class StringExtensions
    {
        public static List<string> CurrencyExceptions = new List<string>
        {
            "TRY",
            "PLN",
            "THB",
            "MXN"
        };
        public static bool ValidateCurrencyExceptions(this string value)
        {
            if(string.IsNullOrEmpty(value)) return false;
            if(CurrencyExceptions.Contains(value)) return false;
            return true;
        }
    }
}
