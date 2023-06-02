using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CurrencyService
{
    public class CurrencyRate
    {
        public string Ticker { get; set; } = null!;

        public decimal Rate { get; set; }

        [JsonIgnore]
        public Currency From { get; set; } = null!;

        [JsonIgnore]
        public Currency To { get; set; } = null!;

        public string FromAlfa3 { get; set; } = null!;

        public string ToAlfa3 { get; set; } = null!;
    }
}
