using System;
using System.Threading;
using IBApi;

namespace AIFOREX
{
    public class VixFetcher : EWrapperImplBase
    {
        private readonly EClientSocket _client;
        private readonly TelegramNotifier _notifier;
        private bool _received = false;

        public VixFetcher(TelegramNotifier notifier)
            : base(new StockDataFetcher(notifier, new EClientSocket(null, new EReaderMonitorSignal())), notifier)
        {
            _notifier = notifier;

            Console.WriteLine("✅ VixFetcher started...");
            _notifier.Send("✅ VixFetcher התחיל לפעול");

            _client = new EClientSocket(this, Signal);
            _client.eConnect("127.0.0.1", 4001, 0);

            var reader = new EReader(_client, Signal);
            reader.Start();

            new Thread(() =>
            {
                while (_client.IsConnected())
                {
                    Signal.waitForSignal();
                    reader.processMsgs();
                }
            })
            { IsBackground = true }.Start();
        }

        public void RequestVix()
        {
            var contract = new Contract
            {
                Symbol = "VIX",
                SecType = "IND",
                Exchange = "CBOE",
                Currency = "USD"
            };

            Console.WriteLine("📡 בקשת מחיר VIX...");
            _notifier.Send("📡 בקשת מחיר VIX...");

            _client.reqMktData(1001, contract, "", false, false, null);
        }

        public override void tickPrice(int tickerId, int field, double price, TickAttrib attribs)
        {
            if (tickerId == 1001 && field == 4 && !_received) // 4 = Last Price
            {
                _received = true;
                string msg = $"📈 מדד VIX: {price:F2}";
                Console.WriteLine(msg);
                _notifier.Send(msg);
                _client.cancelMktData(1001); // הפסקה לאחר קבלת הנתון
            }
        }

        public override void error(int id, int errorCode, string errorMsg)
        {
            Console.WriteLine($"❌ שגיאת VIX: {errorCode} - {errorMsg}");
            _notifier.Send($"❌ שגיאת VIX: {errorCode} - {errorMsg}");
        }

        public override void connectionClosed()
        {
            Console.WriteLine("🔌 החיבור ל־IBKR (VIX) נסגר.");
        }
    }
}
