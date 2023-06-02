using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Xunit;
using Newtonsoft.Json;

namespace CurrencyService.Tests
{
    public class CurrencyConverterTest
    {
        readonly string _CurrCodesFile = "TestResources\\curr-codes.json";
        readonly string _CurrRatesFile = "TestResources\\curr-rates.json";
        readonly string _ConversionRateTestResultFile = "TestResources\\conversion-test-results.json";

        CurrencyConverterFactory _CurrencyConverterFactory;
        List<ConversionRateTestResult> _ConversionRateTestResult;
        List<Currency> _CurrencyCodes;

        public CurrencyConverterTest()
        {
            List<Currency>? currencyCodes = JsonConvert.DeserializeObject<List<Currency>>(File.ReadAllText(_CurrCodesFile));
            List<CurrencyRate>? currencyRates = JsonConvert.DeserializeObject<List<CurrencyRate>>(File.ReadAllText(_CurrRatesFile));

            List<ConversionRateTestResult>? conversionRateTestResult = JsonConvert.DeserializeObject<List<ConversionRateTestResult>>(File.ReadAllText(_ConversionRateTestResultFile));

            if (currencyCodes == null)
                throw new Exception($"{nameof(currencyCodes)} is null");
            if (currencyRates == null)
                throw new Exception($"{nameof(currencyRates)} is null");
            if (conversionRateTestResult == null)
                throw new Exception($"{nameof(conversionRateTestResult)} is null");

            _CurrencyCodes = currencyCodes.ToList();

            _ConversionRateTestResult = conversionRateTestResult;

            _CurrencyConverterFactory = new CurrencyConverterFactory(
                currency: currencyCodes,
                currencyRates: currencyRates);
        }

        [Theory]
        [InlineData("USD")]
        [InlineData("EUR")]
        [InlineData("BYN")]
        public void TestSameCurr(string alfa)
        {
            var curr = _CurrencyCodes.First(c => c.AlphabeticCode == alfa);

            var converter = _CurrencyConverterFactory.GetConverter(curr, curr);

            var rate = converter.Convert(1.0m);

            Assert.Equal(1.0m, rate);
        }

        [Theory]
        [InlineData("USD", "RUB")]
        [InlineData("EUR", "RUB")]
        [InlineData("USD", "CNY")]
        [InlineData("AMD", "RUB")]
        [InlineData("USD", "KZT")]
        public void TestStraightConversion(string alfa1, string alfa2)
        {
            var curr1 = _CurrencyCodes.First(c => c.AlphabeticCode == alfa1);
            var curr2 = _CurrencyCodes.First(c => c.AlphabeticCode == alfa2);

            var curr1curr2Expected = _ConversionRateTestResult.First(r => r.FromAlfa3 == curr1.AlphabeticCode
                && r.ToAlfa3 == curr2.AlphabeticCode);

            var curr1curr2Cvt = _CurrencyConverterFactory.GetConverter(curr1, curr2);
            var curr1curr2Amount = Math.Round(curr1curr2Cvt.Convert(curr1curr2Expected.FromAmount), 2);
            Assert.Equal(curr1curr2Expected.ToAmount, curr1curr2Amount);
        }

        [Theory]
        [InlineData("USD", "RUB")]
        [InlineData("EUR", "RUB")]
        public void TestBasicConversion(string alfa1, string alfa2)
        {
            var curr1 = _CurrencyCodes.First(c => c.AlphabeticCode == alfa1);
            var curr2 = _CurrencyCodes.First(c => c.AlphabeticCode == alfa2);

            var curr1curr2Expected = _ConversionRateTestResult.First(r => r.FromAlfa3 == curr1.AlphabeticCode
                && r.ToAlfa3 == curr2.AlphabeticCode);

            var curr2curr1Expected = _ConversionRateTestResult.First(r => r.FromAlfa3 == curr2.AlphabeticCode
                && r.ToAlfa3 == curr1.AlphabeticCode);


            var curr1curr2Cvt = _CurrencyConverterFactory.GetConverter(curr1, curr2);
            var curr1curr2Amount = Math.Round(curr1curr2Cvt.Convert(curr1curr2Expected.FromAmount), 2);
            Assert.Equal(curr1curr2Expected.ToAmount, curr1curr2Amount);

            var curr2curr1Cvt = _CurrencyConverterFactory.GetConverter(curr2, curr1);
            var curr2curr1Amount = Math.Round(curr2curr1Cvt.Convert(curr2curr1Expected.FromAmount), 2);
            Assert.Equal(curr2curr1Expected.ToAmount, curr2curr1Amount);
        }

        [Theory]
        [InlineData("UGX", "HKD")]
        [InlineData("HKD", "USD")]
        [InlineData("KZT", "HKD")]
        [InlineData("CNY", "EUR")]
        public void TestCrossConversion(string alfa1, string alfa2)
        {
            var curr1 = _CurrencyCodes.First(c => c.AlphabeticCode == alfa1);
            var curr2 = _CurrencyCodes.First(c => c.AlphabeticCode == alfa2);

            var curr1curr2Expected = _ConversionRateTestResult.First(r => r.FromAlfa3 == curr1.AlphabeticCode
                && r.ToAlfa3 == curr2.AlphabeticCode);

            var curr2curr1Expected = _ConversionRateTestResult.First(r => r.FromAlfa3 == curr2.AlphabeticCode
                && r.ToAlfa3 == curr1.AlphabeticCode);


            var curr1curr2Cvt = _CurrencyConverterFactory.GetConverter(curr1, curr2);
            var curr1curr2Amount = Math.Round(curr1curr2Cvt.Convert(curr1curr2Expected.FromAmount), 2);
            Assert.Equal(curr1curr2Expected.ToAmount, curr1curr2Amount);

            var curr2curr1Cvt = _CurrencyConverterFactory.GetConverter(curr2, curr1);
            var curr2curr1Amount = Math.Round(curr2curr1Cvt.Convert(curr2curr1Expected.FromAmount), 2);
            Assert.Equal(curr2curr1Expected.ToAmount, curr2curr1Amount);
        }

        [Fact]
        public void TestAllRates()
        {
            foreach (var result in _ConversionRateTestResult)
            {
                var from = _CurrencyCodes.First(c => c.AlphabeticCode == result.FromAlfa3);
                var to = _CurrencyCodes.First(c => c.AlphabeticCode == result.ToAlfa3);

                var rateCvt = _CurrencyConverterFactory.GetConverter(from, to);
                var rateAmount = Math.Round(rateCvt.Convert(result.FromAmount), 2);

                Assert.Equal(result.ToAmount, rateAmount);
            }
        }
    }
}