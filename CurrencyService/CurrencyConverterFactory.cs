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

        //public CurrencyConverter GetConverter(Currency from, Currency to)
        //{
        //    if (from.AlphabeticCode == to.AlphabeticCode)
        //        return new CurrencyConverter(from, to, value => 1.0m);

        //    var rate = _CurrencyRates.First(r => r.From.AlphabeticCode == from.AlphabeticCode
        //        && r.To.AlphabeticCode == to.AlphabeticCode);

        //    return new CurrencyConverter(from, to, value => value * rate.Rate);
        //}
        public CurrencyConverter GetConverter(Currency from, Currency to)
        {
            if (from.AlphabeticCode == to.AlphabeticCode)
                return new CurrencyConverter(from, to, value => 1.0m);

            // Step 1: Build a conversion graph to store the conversion rates between different currencies
            var conversionGraph = BuildConversionGraph();

            // Step 2: Check if the source currency exists in the conversion graph
            // If not, throw an exception indicating no conversion rate found for the currency
            if (!conversionGraph.ContainsKey(from))
                throw new ArgumentException($"No conversion rate found for currency: {from.AlphabeticCode}");

            // Step 3: Find the conversion rate between the source and target currencies using the conversion graph
            var conversionRate = FindConversionRate(conversionGraph, from, to);

            // Step 4: If no conversion rate is found, throw an exception indicating no rate found between the currencies
            if (conversionRate == null)
                throw new ArgumentException($"No conversion rate found between {from.AlphabeticCode} and {to.AlphabeticCode}");

            // Step 5: Create and return a CurrencyConverter object with the appropriate conversion rate calculation
            return new CurrencyConverter(from, to, value => value * conversionRate.Rate);
        }

        private Dictionary<Currency, Dictionary<Currency, CurrencyRate>> BuildConversionGraph()
        {
            // Step 1: Build a conversion graph using a dictionary of dictionaries
            var graph = new Dictionary<Currency, Dictionary<Currency, CurrencyRate>>();

            // Step 2: Iterate through the CurrencyRate objects and populate the conversion graph
            // with the rates between different currencies
            foreach (var rate in _CurrencyRates)
            {
                if (!graph.ContainsKey(rate.From))
                    graph[rate.From] = new Dictionary<Currency, CurrencyRate>();

                if (!graph.ContainsKey(rate.To))
                    graph[rate.To] = new Dictionary<Currency, CurrencyRate>();

                graph[rate.From][rate.To] = rate;
                graph[rate.To][rate.From] = new CurrencyRate
                {
                    From = rate.To,
                    To = rate.From,
                    Rate = 1.0m / rate.Rate
                };
            }

            // Step 3: Return the built conversion graph
            return graph;
        }

        private CurrencyRate FindConversionRate(Dictionary<Currency, Dictionary<Currency, CurrencyRate>> graph, Currency from, Currency to)
        {
            // Step 1: Find the conversion rate between the source and target currencies using a breadth-first search
            // algorithm with a queue and a visited set

            var visited = new HashSet<Currency>();
            var queue = new Queue<Currency>();
            var path = new Dictionary<Currency, CurrencyRate>();

            visited.Add(from);
            queue.Enqueue(from);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current == to)
                    break;

                foreach (var neighbor in graph[current])
                {
                    if (!visited.Contains(neighbor.Key))
                    {
                        visited.Add(neighbor.Key);
                        queue.Enqueue(neighbor.Key);
                        path[neighbor.Key] = graph[current][neighbor.Key];
                    }
                }
            }

            if (!path.ContainsKey(to))
                return null;

            var rate = 1.0m;
            var currency = to;

            while (currency != from)
            {
                var edge = path[currency];
                rate *= edge.Rate;
                currency = edge.From;
            }

            // Step 2: Return the found conversion rate
            return new CurrencyRate
            {
                From = from,
                To = to,
                Rate = rate
            };
        }
    }
}
    



