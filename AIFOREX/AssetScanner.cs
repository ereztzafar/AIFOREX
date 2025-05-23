using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using IBApi;

namespace AIFOREX
{
    public class AssetScanner
    {
        private readonly TelegramNotifier _notifier;
        private readonly StockDataFetcher _fetcher;
        private readonly AIDecisionEngine _ai = new(); // שימוש במנוע החדש
        private readonly SqlTradeLogger _logger = new();
        private readonly TradeManager _tradeManager;
        private readonly bool _isLive;

        public AssetScanner(TelegramNotifier notifier, EClientSocket client, TradeManager tradeManager = null, bool isLive = false)
        {
            _notifier = notifier;
            _fetcher = new StockDataFetcher(notifier, client);
            _tradeManager = tradeManager;
            _isLive = isLive;
        }

        public async Task ScanAllAsync()
        {
            await ScanFromFileAsync("SP500.csv", "SP500");
            await ScanFromFileAsync("fx.csv", "FOREX");
            await ScanFromFileAsync("tase.csv", "TASE");

            _notifier.Send("✅ All categories scanned.");
        }

        private async Task ScanFromFileAsync(string fileName, string assetType)
        {
            if (!File.Exists(fileName))
            {
                Console.WriteLine($"[WARN] File not found: {fileName}");
                _notifier.Send($"❌ File not found: {fileName}");
                return;
            }

            var symbols = File.ReadAllLines(fileName)
                              .Where(line => !string.IsNullOrWhiteSpace(line))
                              .ToList();

            foreach (var rawSymbol in symbols)
            {
                string symbol = rawSymbol.Trim();

                if (symbol.Contains(".")) continue;
                if (_logger.WasScannedToday(assetType, symbol)) continue;

                try
                {
                    Console.WriteLine($"🔍 Scanning: {symbol}");

                    _fetcher.RequestHistoricalDaily(symbol);
                    _fetcher.RequestHistoricalIntraday(symbol);

                    await Task.Delay(6000); // זמן המתנה לטעינת נתונים

                    var daily = _fetcher.GetDailyData(symbol);
                    var hourly = _fetcher.GetHourlyData(symbol);

                    if (daily.Count < 200 || hourly.Count < 30)
                    {
                        Console.WriteLine($"[SKIP] Insufficient data for {symbol}");
                        continue;
                    }

                    decimal ma50 = (decimal)TechnicalIndicators.CalculateSMA(daily, 50).Last();
                    decimal ma150 = (decimal)TechnicalIndicators.CalculateSMA(daily, 150).Last();
                    decimal ma200 = (decimal)TechnicalIndicators.CalculateSMA(daily, 200).Last();
                    decimal rsi = (decimal)TechnicalIndicators.CalculateRSI(hourly, 14).Last();
                    var macdResult = TechnicalIndicators.CalculateMACD(hourly);
                    decimal macd = (decimal)macdResult.macd.Last();
                    decimal signal = (decimal)macdResult.signal.Last();
                    decimal currentPrice = (decimal)daily.Last();
                    decimal vix = 18; // VIX קבוע לבדיקה

                    var input = new DecisionInputBase(
                        symbol, rsi, macd, signal, currentPrice, vix, ma50, ma150, ma200
                    );

                    Console.WriteLine($"[DEBUG] שליחת נתונים ל-AI עבור {symbol}");
                    var decision = _ai.Decide(input);
                    string indicators = $"RSI: {rsi:F2}, MACD: {macd:F2}, Signal: {signal:F2}";

                    if (decision.Action != "HOLD")
                    {
                        _notifier.Send($"📣 {symbol}: {decision.Action} | {decision.Reason}\n📊 {indicators}");
                        await Task.Delay(1000);
                    }

                    _logger.LogTrade(assetType, symbol, (double)currentPrice, 10, 0.0, decision.Action, decision.Reason, indicators);

                    if (_isLive && (decision.Action == "BUY" || decision.Action == "SELL"))
                    {
                        _tradeManager?.PlaceOrder(assetType, symbol, decision.Action, 10, (double)currentPrice, (double)rsi, (double)macd);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Failed to scan {symbol}: {ex.Message}");
                    _notifier.Send($"❌ Error scanning {symbol}: {ex.Message}");
                }
            }

            Console.WriteLine($"✅ Finished scanning: {assetType}");
            _notifier.Send($"✅ Finished scanning {assetType}");
        }
    }

    internal class MyAIDecisionEngine
    {
    }
}
