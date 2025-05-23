using System;
using System.Threading.Tasks;
using IBApi;

namespace AIFOREX
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("🚀 התחלת הרצת הרובוט");

            // יצירת רכיבי תקשורת והתראות
            var notifier = new TelegramNotifier();
            var signal = new EReaderMonitorSignal();
            var wrapper = new EWrapperImplBase(null, notifier);
            var client = new EClientSocket(wrapper, signal);

            // התחברות ל־IBKR (TWS או IB Gateway)
            client.eConnect("127.0.0.1", 4001, 0);

            // יצירת fetcher
            var fetcher = new StockDataFetcher(notifier, client);
            typeof(EWrapperImplBase).GetField("_fetcher", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(wrapper, fetcher);

            // יצירת ניהול פורטפוליו וטרייד
            var portfolioManager = new PortfolioManager();
            var tradeManager = new TradeManager(client, notifier, portfolioManager, isLive: true);

            // יצירת סורק והפעלתו בצורה אסינכרונית
            var scanner = new AssetScanner(notifier, client, tradeManager, isLive: true);
            await scanner.ScanAllAsync();

            // סיום
            Console.WriteLine("✅ הסריקה הסתיימה.");
            notifier.Send("✅ הרצת הסריקה הסתיימה.");

            Console.WriteLine("לחץ על מקש כלשהו כדי לצאת...");
            Console.ReadKey();
        }
    }
}
