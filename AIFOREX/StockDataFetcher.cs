using System;
using System.Collections.Generic;
using IBApi;

namespace AIFOREX
{
    public class StockDataFetcher
    {
        public EClientSocket ClientSocket => _client;

        private readonly TelegramNotifier _notifier;
        private readonly EClientSocket _client;

        private readonly Dictionary<int, string> _requestSymbolMap = new();
        private readonly Dictionary<string, List<double>> _dailyData = new();
        private readonly Dictionary<string, List<double>> _hourlyData = new();

        private int _requestIdCounter = 1000;

        public StockDataFetcher(TelegramNotifier notifier, EClientSocket client)
        {
            _notifier = notifier;
            _client = client;
        }

        public void RequestHistoricalDaily(string symbol)
        {
            int reqId = _requestIdCounter++;
            _requestSymbolMap[reqId] = symbol;

            var contract = CreateContract(symbol);
            _client.reqHistoricalData(
                reqId,
                contract,
                "",
                "3 Y",
                "1 day",
                "TRADES",
                1,
                1,
                false,
                null
            );

            Console.WriteLine($"[DAILY] Requested historical data for {symbol} (reqId {reqId})");
            _notifier?.Send($"📊 Daily request: {symbol}");
        }

        public void RequestHistoricalIntraday(string symbol)
        {
            int reqId = _requestIdCounter++;
            _requestSymbolMap[reqId] = symbol;

            var contract = CreateContract(symbol);
            _client.reqHistoricalData(
                reqId,
                contract,
                "",
                "10 D",
                "1 hour",
                "TRADES",
                1,
                1,
                false,
                null
            );

            Console.WriteLine($"[INTRADAY] Requested hourly data for {symbol} (reqId {reqId})");
            _notifier?.Send($"📈 Intraday request: {symbol}");
        }

        public void OnHistoricalDataReceived(int reqId, double close)
        {
            if (!_requestSymbolMap.TryGetValue(reqId, out var symbol))
                return;

            var list = reqId % 2 == 0 ? GetOrCreate(_dailyData, symbol) : GetOrCreate(_hourlyData, symbol);
            list.Add(close);
        }

        public List<double> GetDailyData(string symbol) => _dailyData.ContainsKey(symbol) ? _dailyData[symbol] : new();
        public List<double> GetHourlyData(string symbol) => _hourlyData.ContainsKey(symbol) ? _hourlyData[symbol] : new();

        private List<double> GetOrCreate(Dictionary<string, List<double>> dict, string symbol)
        {
            if (!dict.ContainsKey(symbol))
                dict[symbol] = new List<double>();
            return dict[symbol];
        }

        private Contract CreateContract(string symbol)
        {
            return new Contract
            {
                Symbol = symbol,
                SecType = "STK",
                Exchange = "SMART",
                Currency = "USD"
            };
        }
    }
}
