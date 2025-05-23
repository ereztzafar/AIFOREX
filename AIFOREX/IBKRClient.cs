using System;
using System.Threading;
using IBApi;

namespace AIFOREX
{
    public class IBKRClient : EWrapperImplBase
    {
        private readonly TelegramNotifier _notifier;

        public IBKRClient(TelegramNotifier notifier)
            : base(new StockDataFetcher(notifier, new EClientSocket(null, new EReaderMonitorSignal())), notifier)
        {
            _notifier = notifier;

            Console.WriteLine("☑️ IBKRClient started...");
            _notifier.Send("☑️ IBKRClient התחיל לפעול");

            try
            {
                Console.WriteLine("🔗 Connecting to IBKR...");
                _notifier.Send("🔗 התחברות ל־IBKR...");

                ClientSocket = new EClientSocket(this, Signal);
                ClientSocket.eConnect("127.0.0.1", 7497, 0);

                var reader = new EReader(ClientSocket, Signal);
                reader.Start();

                new Thread(() =>
                {
                    while (ClientSocket.IsConnected())
                    {
                        Signal.waitForSignal();
                        reader.processMsgs();
                    }
                })
                { IsBackground = true }.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ שגיאה בהתחברות: {ex.Message}");
                _notifier.Send($"❌ שגיאה בהתחברות: {ex.Message}");
            }
        }
    }
}
