using System;
using System.Data.SQLite;
using System.IO;

namespace AIFOREX
{
    public class SqlTradeLogger
    {
        private const string DbFile = "trades.db";
        private const string ConnectionString = "Data Source=trades.db;Version=3;";

        public SqlTradeLogger()
        {
            if (!File.Exists(DbFile))
            {
                SQLiteConnection.CreateFile(DbFile);
            }

            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            string createTable = @"
                CREATE TABLE IF NOT EXISTS trades (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    date TEXT NOT NULL,
                    asset TEXT NOT NULL,
                    symbol TEXT NOT NULL,
                    price REAL,
                    quantity INTEGER,
                    pnl REAL,
                    action TEXT,
                    reason TEXT,
                    indicators TEXT
                );
            ";

            using var command = new SQLiteCommand(createTable, connection);
            command.ExecuteNonQuery();
        }

        public void LogTrade(string assetType, string symbol, double price, int quantity, double pnl,
                             string action, string reason, string indicators)
        {
            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            string insert = @"
                INSERT INTO trades (date, asset, symbol, price, quantity, pnl, action, reason, indicators)
                VALUES (@date, @asset, @symbol, @price, @quantity, @pnl, @action, @reason, @indicators);
            ";

            using var cmd = new SQLiteCommand(insert, connection);
            cmd.Parameters.AddWithValue("@date", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@asset", assetType);
            cmd.Parameters.AddWithValue("@symbol", symbol);
            cmd.Parameters.AddWithValue("@price", price);
            cmd.Parameters.AddWithValue("@quantity", quantity);
            cmd.Parameters.AddWithValue("@pnl", pnl);
            cmd.Parameters.AddWithValue("@action", action);
            cmd.Parameters.AddWithValue("@reason", reason);
            cmd.Parameters.AddWithValue("@indicators", indicators);

            cmd.ExecuteNonQuery();
        }

        public bool WasScannedToday(string assetType, string symbol)
        {
            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            string today = DateTime.UtcNow.ToString("yyyy-MM-dd");
            string query = @"
                SELECT COUNT(*) FROM trades
                WHERE asset = @asset AND symbol = @symbol AND date LIKE @date || '%';
            ";

            using var cmd = new SQLiteCommand(query, connection);
            cmd.Parameters.AddWithValue("@asset", assetType);
            cmd.Parameters.AddWithValue("@symbol", symbol);
            cmd.Parameters.AddWithValue("@date", today);

            long count = (long)cmd.ExecuteScalar();
            return count > 0;
        }
    }
}

