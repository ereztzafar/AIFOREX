using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace AIFOREX
{
    public class CsvHistoryStorage
    {
        private readonly string _basePath = "history";

        public CsvHistoryStorage()
        {
            if (!Directory.Exists(_basePath))
                Directory.CreateDirectory(_basePath);
        }

        public void SaveDailyData(string category, string symbol, List<double> closePrices)
        {
            string filePath = Path.Combine(_basePath, $"{category}_{symbol}_daily.csv");
            using var writer = new StreamWriter(filePath, false);

            writer.WriteLine("Date,Close");
            var date = DateTime.Today.AddDays(-closePrices.Count + 1);
            foreach (var price in closePrices)
            {
                writer.WriteLine($"{date:yyyy-MM-dd},{price.ToString(CultureInfo.InvariantCulture)}");
                date = date.AddDays(1);
            }
        }

        public bool WasScannedToday(string category, string symbol)
        {
            string filePath = Path.Combine(_basePath, $"{category}_{symbol}_daily.csv");
            if (!File.Exists(filePath))
                return false;

            var lastLine = File.ReadLines(filePath).LastOrDefault();
            if (lastLine == null) return false;

            var parts = lastLine.Split(',');
            if (parts.Length < 2) return false;

            if (DateTime.TryParse(parts[0], out DateTime lastDate))
                return lastDate.Date == DateTime.Today;

            return false;
        }

        public List<double> LoadDailyData(string category, string symbol)
        {
            string filePath = Path.Combine(_basePath, $"{category}_{symbol}_daily.csv");
            var prices = new List<double>();

            if (!File.Exists(filePath)) return prices;

            var lines = File.ReadAllLines(filePath).Skip(1); // Skip header
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length == 2 && double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double price))
                {
                    prices.Add(price);
                }
            }

            return prices;
        }
    }
}

