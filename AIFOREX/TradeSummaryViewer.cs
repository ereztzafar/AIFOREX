using System;
using System.Data.SQLite;

namespace AIFOREX
{
    public class TradeSummaryViewer
    {
        private const string ConnectionString = "Data Source=trades.db;Version=3;";

        public void PrintAllTrades()
        {
            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            string query = "SELECT date, asset, symbol, price, quantity, pnl, action FROM trades ORDER BY date DESC";

            using var cmd = new SQLiteCommand(query, connection);
            using var reader = cmd.ExecuteReader();

            Console.WriteLine("📋 Trade History:");
            Console.WriteLine("---------------------------------------------------------------");
            Console.WriteLine("Date\t\tAsset\tSymbol\tPrice\tQty\tPnL\tAction");

            while (reader.Read())
            {
                string date = reader.GetString(0);
                string asset = reader.GetString(1);
                string symbol = reader.GetString(2);
                double price = reader.GetDouble(3);
                int qty = reader.GetInt32(4);
                double pnl = reader.GetDouble(5);
                string action = reader.GetString(6);

                Console.WriteLine($"{date}\t{asset}\t{symbol}\t{price:F2}\t{qty}\t{pnl:F2}\t{action}");
            }

            Console.WriteLine("---------------------------------------------------------------");
        }

        public void PrintActionSummary()
        {
            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            string query = @"
                SELECT action, COUNT(*) as count, SUM(pnl) as total_pnl
                FROM trades
                GROUP BY action
                ORDER BY action;
            ";

            using var cmd = new SQLiteCommand(query, connection);
            using var reader = cmd.ExecuteReader();

            Console.WriteLine("📊 Summary by Action:");
            Console.WriteLine("----------------------------------");
            Console.WriteLine("Action\tCount\tTotal PnL");

            while (reader.Read())
            {
                string action = reader.GetString(0);
                int count = reader.GetInt32(1);
                double totalPnl = reader.IsDBNull(2) ? 0 : reader.GetDouble(2);

                Console.WriteLine($"{action}\t{count}\t{totalPnl:F2}");
            }

            Console.WriteLine("----------------------------------");
        }
    }
}

