using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyService
{
    public class CurrencyConverter
    {
        public Currency From { get; set; }

        public Currency To { get; set; }

        public Func<decimal, decimal> Convert { get; set; }

        public CurrencyConverter(Currency from, Currency to, Func<decimal, decimal> convert)
        {
            From = from;
            To = to;
            Convert = convert;
        }
    }
}
