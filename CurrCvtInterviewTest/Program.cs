using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CurrencyService;
using CurrencyService.Tests;

namespace CurrCvtInterviewTest
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            List<Currency> currencyCodes = JsonConvert.DeserializeObject<List<Currency>>(File.ReadAllText("Resources\\curr-codes.json"))!;
            List<CurrencyRate> currencyRates = JsonConvert.DeserializeObject<List<CurrencyRate>>(File.ReadAllText("Resources\\curr-rates.json"))!;

            CurrencyConverterFactory currencyConverterFactory = new CurrencyConverterFactory(
                currency: currencyCodes,
                currencyRates: currencyRates);

            var usd = currencyCodes.First(c => c.AlphabeticCode == "USD");
            var rub = currencyCodes.First(c => c.AlphabeticCode == "RUB");
            var cvt1 = currencyConverterFactory.GetConverter(usd, rub);
            var rate1 = cvt1.Convert(1.0m);

            var ugx = currencyCodes.First(c => c.AlphabeticCode == "UGX");
            var hkd = currencyCodes.First(c => c.AlphabeticCode == "HKD");
            var cvt2 = currencyConverterFactory.GetConverter(ugx, hkd);
            var rate2 = cvt1.Convert(1.0m);
        }
    }
}