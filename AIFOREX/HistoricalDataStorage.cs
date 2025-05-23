using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using OfficeOpenXml;

namespace AIFOREX
{
    public class HistoricalDataStorage
    {
        private readonly string _filePath = "MarketHistory.xlsx";

        public HistoricalDataStorage()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            if (!File.Exists(_filePath))
            {
                using var p = new ExcelPackage(new FileInfo(_filePath));
                p.Workbook.Worksheets.Add("TASE");
                p.Workbook.Worksheets.Add("FX");
                p.Workbook.Worksheets.Add("SP500");
                p.Save();
            }
        }

        public void SaveHistory(string category, string symbol, List<(DateTime date, double close)> history)
        {
            using var package = new ExcelPackage(new FileInfo(_filePath));
            var ws = package.Workbook.Worksheets[category] ?? package.Workbook.Worksheets.Add(category);

            int col = FindOrCreateColumn(ws, symbol);

            for (int i = 0; i < history.Count; i++)
            {
                var row = i + 2;
                ws.Cells[row, 1].Value = history[i].date.ToString("yyyy-MM-dd");
                ws.Cells[row, col].Value = history[i].close;
            }

            package.Save();
        }

        public List<(DateTime date, double close)> LoadHistory(string category, string symbol)
        {
            var result = new List<(DateTime, double)>();
            if (!File.Exists(_filePath)) return result;

            using var package = new ExcelPackage(new FileInfo(_filePath));
            var ws = package.Workbook.Worksheets[category];
            if (ws == null) return result;

            int col = FindColumn(ws, symbol);
            if (col == -1) return result;

            int row = 2;
            while (true)
            {
                var dateVal = ws.Cells[row, 1].Value?.ToString();
                var closeVal = ws.Cells[row, col].Value?.ToString();
                if (string.IsNullOrWhiteSpace(dateVal)) break;

                if (DateTime.TryParse(dateVal, out DateTime date) &&
                    double.TryParse(closeVal, out double close))
                {
                    result.Add((date, close));
                }
                row++;
            }

            return result;
        }

        private int FindOrCreateColumn(ExcelWorksheet ws, string symbol)
        {
            int col = 2;
            while (true)
            {
                var val = ws.Cells[1, col].Value?.ToString();
                if (string.IsNullOrWhiteSpace(val))
                {
                    ws.Cells[1, col].Value = symbol;
                    return col;
                }

                if (val.Equals(symbol, StringComparison.OrdinalIgnoreCase))
                    return col;

                col++;
            }
        }

        private int FindColumn(ExcelWorksheet ws, string symbol)
        {
            int col = 2;
            while (true)
            {
                var val = ws.Cells[1, col].Value?.ToString();
                if (string.IsNullOrWhiteSpace(val)) return -1;
                if (val.Equals(symbol, StringComparison.OrdinalIgnoreCase)) return col;
                col++;
            }
        }
    }
}
