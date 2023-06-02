using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyService.Tests
{
    public class ConversionRateTestResult
    {
        public string FromAlfa3 { get; set; } = null!;

        public string ToAlfa3 { get; set; } = null!;

        public decimal FromAmount { get; set; }

        public decimal ToAmount { get; set; }
    }
}
