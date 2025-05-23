using System;
using System.IO;
using OfficeOpenXml;

namespace AIFOREX
{
    public class ExcelLogger
    {
        private const string LogFile = "trades_log.xlsx";

        public ExcelLogger()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public void LogTrade(string assetType, string symbol, double price, int quantity, double pnl,
                             string action, string reason, string indicators)
        {
            if (!File.Exists(LogFile))
            {
                using var package = new ExcelPackage();
                package.Workbook.Worksheets.Add("Log");
                package.SaveAs(new FileInfo(LogFile));
            }

            using var file = new ExcelPackage(new FileInfo(LogFile));
            var sheet = file.Workbook.Worksheets["Log"] ?? file.Workbook.Worksheets.Add("Log");

            int row = sheet.Dimension?.Rows + 1 ?? 2;

            if (row == 2)
            {
                sheet.Cells[1, 1].Value = "Date";
                sheet.Cells[1, 2].Value = "Asset";
                sheet.Cells[1, 3].Value = "Symbol";
                sheet.Cells[1, 4].Value = "Price";
                sheet.Cells[1, 5].Value = "Quantity";
                sheet.Cells[1, 6].Value = "PnL";
                sheet.Cells[1, 7].Value = "Action";
                sheet.Cells[1, 8].Value = "Reason";
                sheet.Cells[1, 9].Value = "Indicators";
            }

            sheet.Cells[row, 1].Value = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            sheet.Cells[row, 2].Value = assetType;
            sheet.Cells[row, 3].Value = symbol;
            sheet.Cells[row, 4].Value = price;
            sheet.Cells[row, 5].Value = quantity;
            sheet.Cells[row, 6].Value = pnl;
            sheet.Cells[row, 7].Value = action;
            sheet.Cells[row, 8].Value = reason;
            sheet.Cells[row, 9].Value = indicators;

            file.Save();
        }

        public bool WasScannedToday(string assetType, string symbol)
        {
            if (!File.Exists(LogFile)) return false;

            using var file = new ExcelPackage(new FileInfo(LogFile));
            var sheet = file.Workbook.Worksheets["Log"];
            if (sheet == null || sheet.Dimension == null) return false;

            int rows = sheet.Dimension.Rows;
            string today = DateTime.UtcNow.ToString("yyyy-MM-dd");

            for (int i = 2; i <= rows; i++)
            {
                string date = sheet.Cells[i, 1].Text;
                string asset = sheet.Cells[i, 2].Text;
                string sym = sheet.Cells[i, 3].Text;

                if (date.StartsWith(today) && asset == assetType && sym == symbol)
                    return true;
            }

            return false;
        }
    }
}
