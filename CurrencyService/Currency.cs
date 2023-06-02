using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyService
{
    public class Currency
    {
        [JsonProperty("Currency")]
        public string? CurrencyName { get; set; }

        public string AlphabeticCode { get; set; } = null!;

        public string NumericCode { get; set; } = null!;

        public string? MinorUnit { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(CurrencyName))
                return $"{AlphabeticCode}";
            else
                return $"{AlphabeticCode}: {CurrencyName}";
        }
    }

}
