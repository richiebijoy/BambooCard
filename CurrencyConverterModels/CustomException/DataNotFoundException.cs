using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverterModels.CustomException
{
    public class DataNotFoundException : Exception
    {
        public DataNotFoundException(string message) : base(message)
        {
        }
    }
    public class BadRequestException : Exception
    {
        public BadRequestException() : base("Bad Request!")
        {
        }
        public BadRequestException(string message) : base(message)
        {
        }
    }
}
