using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FinancialPlanner.Models;
using Newtonsoft.Json;

namespace FinancialPlanner.ConsoleApp.Services
{
    public class CurrencyService
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly Dictionary<string, decimal> _cache = new();
        private DateTime _lastUpdate = DateTime.MinValue;

        public List<Currency> GetAvailableCurrencies()
        {
            return new List<Currency>
            {
                new Currency { Code = "RUB", Name = "Российский рубль", Symbol = "₽" },
                new Currency { Code = "USD", Name = "Доллар США", Symbol = "$" },
                new Currency { Code = "EUR", Name = "Евро", Symbol = "€" },
                new Currency { Code = "GBP", Name = "Фунт стерлингов", Symbol = "£" },
                new Currency { Code = "JPY", Name = "Японская йена", Symbol = "¥" },
                new Currency { Code = "CNY", Name = "Китайский юань", Symbol = "¥" },
                new Currency { Code = "KRW", Name = "Южнокорейская вона", Symbol = "₩" }
            };
        }

        public async Task<decimal> GetRate(string from, string to)
        {
            if (from == to) return 1m;

            var key = $"{from}_{to}";
            if (_cache.ContainsKey(key) && DateTime.Now - _lastUpdate < TimeSpan.FromHours(1))
                return _cache[key];

            try
            {
                var json = await _client.GetStringAsync($"https://api.exchangerate-api.com/v4/latest/{from}");
                var data = JsonConvert.DeserializeObject<RateResponse>(json);
                
                if (data?.Rates != null && data.Rates.ContainsKey(to))
                {
                    var rate = data.Rates[to];
                    _cache[key] = rate;
                    _lastUpdate = DateTime.Now;
                    return rate;
                }
            }
            catch { }

            return GetFallback(from, to);
        }

        private decimal GetFallback(string from, string to)
        {
            var rates = new Dictionary<string, decimal>
            {
                { "USD_RUB", 90m }, { "EUR_RUB", 98m }, { "GBP_RUB", 115m },
                { "JPY_RUB", 0.6m }, { "CNY_RUB", 12.5m }, { "KRW_RUB", 0.07m }
            };

            var key = $"{from}_{to}";
            if (rates.ContainsKey(key)) return rates[key];
            
            var reverse = $"{to}_{from}";
            if (rates.ContainsKey(reverse)) return 1m / rates[reverse];
            
            return 1m;
        }

        private class RateResponse
        {
            [JsonProperty("rates")]
            public Dictionary<string, decimal>? Rates { get; set; }
        }
    }
}
