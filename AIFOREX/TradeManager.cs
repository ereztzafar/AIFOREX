using System;
using IBApi;

namespace AIFOREX
{
    public class TradeManager
    {
        public readonly EClientSocket _client;
        private readonly TelegramNotifier _notifier;
        private readonly PortfolioManager _portfolio;
        private readonly bool isLive;

        public TradeManager(EClientSocket client, TelegramNotifier notifier, PortfolioManager portfolio, bool isLive)
        {
            _client = client;
            _notifier = notifier;
            _portfolio = portfolio;
            this.isLive = isLive;
        }

        public void ExecuteTrade(string action, string symbol, double price, string quantityStr, double? stopLoss = null, double? takeProfit = null)
        {
            double quantity = Convert.ToDouble(quantityStr);
            int orderId = new Random().Next(100000, 999999);
            var contract = CreateStockContract(symbol);

            if (isLive)
            {
                // פקודת שוק רגילה
                var marketOrder = new Order
                {
                    Action = action.ToUpper(),
                    OrderType = "MKT",
                    TotalQuantity = quantity,
                    Tif = "GTC"
                };
                _client.placeOrder(orderId, contract, marketOrder);
                _notifier.Send($"שליחת פקודת {action.ToUpper()} עבור {symbol} בכמות: {quantityStr}");

                // Stop Loss
                if (stopLoss.HasValue)
                {
                    int stopOrderId = orderId + 1;
                    var stopOrder = new Order
                    {
                        Action = action.ToUpper() == "BUY" ? "SELL" : "BUY",
                        OrderType = "STP",
                        AuxPrice = stopLoss.Value,
                        TotalQuantity = quantity,
                        Tif = "GTC"
                    };
                    _client.placeOrder(stopOrderId, contract, stopOrder);
                    _notifier.Send($"🛑 Stop Loss ב-{stopLoss.Value} עבור {symbol}");
                }

                // Take Profit
                if (takeProfit.HasValue)
                {
                    int tpOrderId = orderId + 2;
                    var tpOrder = new Order
                    {
                        Action = action.ToUpper() == "BUY" ? "SELL" : "BUY",
                        OrderType = "LMT",
                        LmtPrice = takeProfit.Value,
                        TotalQuantity = quantity,
                        Tif = "GTC"
                    };
                    _client.placeOrder(tpOrderId, contract, tpOrder);
                    _notifier.Send($"🎯 Take Profit ב-{takeProfit.Value} עבור {symbol}");
                }
            }
            else
            {
                _notifier.Send($"[סימולציה] פקודת {action.ToUpper()} עבור {symbol} בכמות: {quantityStr}");
            }

            if (_portfolio != null)
            {
                if (action.ToUpper() == "BUY")
                    _portfolio.RegisterBuy(symbol, price, quantity);
                else if (action.ToUpper() == "SELL")
                    _portfolio.RegisterSell(symbol, price);
            }
        }

        private Contract CreateStockContract(string symbol)
        {
            return new Contract
            {
                Symbol = symbol,
                SecType = "STK",
                Exchange = "SMART",
                Currency = "USD"
            };
        }

        public void PlaceOrder(string assetType, string symbol, string action, int quantity, double price, double rsi, double macd)
        {
            // דוגמה פשוטה: SL ב-2% מתחת למחיר, TP ב-3% מעל
            double? stopLoss = action.ToUpper() == "BUY" ? price * 0.98 : price * 1.02;
            double? takeProfit = action.ToUpper() == "BUY" ? price * 1.03 : price * 0.97;

            ExecuteTrade(action, symbol, price, quantity.ToString(), stopLoss, takeProfit);
        }
    }
}
