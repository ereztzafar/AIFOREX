using System;
using System.Linq;
using System.Threading;
using IBApi;

namespace AIFOREX
{
    public class PortfolioChecker
    {
        private readonly StockDataFetcher _fetcher;
        private readonly TelegramNotifier _notifier;
        private readonly double _vix;

        public PortfolioChecker(StockDataFetcher fetcher, TelegramNotifier notifier, double vix)
        {
            _fetcher = fetcher;
            _notifier = notifier;
            _vix = vix;
        }

        public void CheckPortfolio()
        {
            // בקשת מידע על הפוזיציות מ-IBKR
            _fetcher.ClientSocket.reqPositions();

            new Thread(() =>
            {
                Thread.Sleep(3000);

                // דוגמה עם מניית AAPL
                string symbol = "AAPL";
                double avgCost = 200.0;
                int quantity = 10;

                // שימוש בשיטה תקינה
                var dailyPrices = _fetcher.GetDailyData(symbol);
                double currentPrice = dailyPrices.LastOrDefault();

                if (currentPrice <= 0) return;

                double pnl = (currentPrice - avgCost) * quantity;
                double pnlPercent = ((currentPrice - avgCost) / avgCost) * 100.0;

                double stopLossPercent = _vix > 25 ? 1.0 : (_vix > 20 ? 2.0 : 3.0);

                if (pnlPercent <= -stopLossPercent)
                {
                    string message = $"⚠️ Stop Loss Triggered on {symbol}\nCurrent Price: {currentPrice}, PnL%: {pnlPercent:F2}";
                    _notifier.Send(message);
                }

            }).Start();
        }
    }
}
