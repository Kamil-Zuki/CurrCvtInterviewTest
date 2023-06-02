using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyService
{
    public class CurrencyConverterFactory
    {
        List<Currency> _CurrencyCodes;
        List<CurrencyRate> _CurrencyRates;

        public CurrencyConverterFactory(IEnumerable<Currency> currency, IEnumerable<CurrencyRate> currencyRates)
        {
            _CurrencyCodes = currency.ToList();
            _CurrencyRates = currencyRates.ToList();

            foreach (var r in _CurrencyRates)
            {
                r.From = _CurrencyCodes.First(c => c.AlphabeticCode == r.FromAlfa3);
                r.To = _CurrencyCodes.First(c => c.AlphabeticCode == r.ToAlfa3);
            }
        }

        public CurrencyConverter GetConverter(Currency from, Currency to)
        {
            if (from.AlphabeticCode == to.AlphabeticCode)
                return new CurrencyConverter(from, to, value => 1.0m);

            var rate = _CurrencyRates.First(r => r.From.AlphabeticCode == from.AlphabeticCode
                && r.To.AlphabeticCode == to.AlphabeticCode);

            return new CurrencyConverter(from, to, value => value * rate.Rate);
        }
    }
}
