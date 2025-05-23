using System;
using System.Collections.Generic;
using IBApi;

namespace AIFOREX
{
    public class EWrapperImplBase : EWrapper
    {
        public EClientSocket ClientSocket { get; protected set; }
        public EReaderSignal Signal { get; } = new EReaderMonitorSignal();
        private readonly TelegramNotifier _notifier;
        private readonly StockDataFetcher _fetcher;
        private readonly Dictionary<int, string> _requestSymbolMap = new();

        public EWrapperImplBase(StockDataFetcher fetcher, TelegramNotifier notifier = null)
        {
            _notifier = notifier;
            _fetcher = fetcher;
            ClientSocket = new EClientSocket(this, Signal);
        }

        public void RegisterRequest(int reqId, string symbol)
        {
            _requestSymbolMap[reqId] = symbol;
        }

        public virtual void historicalData(int reqId, Bar bar)
        {
            if (_requestSymbolMap.TryGetValue(reqId, out string symbol))
            {
                Console.WriteLine($"[DATA] {symbol} | {bar.Time} | Close: {bar.Close}");
                _fetcher?.OnHistoricalDataReceived(reqId, bar.Close);
            }
            else
            {
                Console.WriteLine($"[WARN] Unknown reqId {reqId} for bar: {bar.Time}");
            }
        }

        public virtual void historicalDataEnd(int reqId, string start, string end)
        {
            if (_requestSymbolMap.TryGetValue(reqId, out string symbol))
            {
                Console.WriteLine($"[DONE] Historical data complete for {symbol} (reqId {reqId})");
            }
            else
            {
                Console.WriteLine($"[DONE] Historical data complete (unknown reqId {reqId})");
            }
        }

        public virtual void orderStatus(int orderId, string status, double filled, double remaining,
                                        double avgFillPrice, int permId, int parentId,
                                        double lastFillPrice, int clientId, string whyHeld, double mktCapPrice)
        {
            string msg = $"[ORDER] ID={orderId}, Status={status}, Filled={filled}, AvgPrice={avgFillPrice}";
            Console.WriteLine(msg);
            _notifier?.Send(msg);
        }

        public virtual void error(int id, int errorCode, string errorMsg)
        {
            string msg = $"[ERROR] {errorCode}: {errorMsg}";
            Console.WriteLine(msg);
            _notifier?.Send(msg);
        }

        public virtual void error(Exception e)
        {
            Console.WriteLine("[EXCEPTION] " + e.Message);
        }

        public virtual void error(string str)
        {
            Console.WriteLine("[ERROR] " + str);
        }

        public virtual void connectionClosed()
        {
            Console.WriteLine("[INFO] Connection to IBKR closed.");
        }

        public virtual void connectAck()
        {
            if (ClientSocket.AsyncEConnect)
                ClientSocket.startApi();
        }

        // Empty implementations for other EWrapper methods
        public virtual void tickPrice(int tickerId, int field, double price, TickAttrib attrib) { }
        public virtual void tickSize(int tickerId, int field, int size) { }
        public virtual void nextValidId(int orderId) { }
        public virtual void managedAccounts(string accountsList) { }
        public virtual void openOrder(int orderId, Contract contract, IBApi.Order order, OrderState orderState) { }
        public virtual void openOrderEnd() { }
        public virtual void fundamentalData(int reqId, string data) { }
        public virtual void currentTime(long time) { }
        public virtual void position(string account, Contract contract, double pos, double avgCost) { }
        public virtual void positionEnd() { }
        public virtual void accountDownloadEnd(string account) { }

        // Remaining interface methods left unimplemented for brevity
        
        public void tickEFP(int tickerId, int tickType, double basisPoints, string formattedBasisPoints, double impliedFuture, int holdDays, string futureLastTradeDate, double dividendImpact, double dividendsToLastTradeDate)
        {
            // TODO: implement method logic
        }

        public void deltaNeutralValidation(int reqId, UnderComp underComp)
        {
            // TODO: implement method logic
        }

        public void tickOptionComputation(int tickerId, int field, double impliedVolatility, double delta, double optPrice, double pvDividend, double gamma, double vega, double theta, double undPrice)
        {
            // TODO: implement method logic
        }

        public void tickSnapshotEnd(int tickerId)
        {
            // TODO: implement method logic
        }

        public void accountSummary(int reqId, string account, string tag, string value, string currency)
        {
            // TODO: implement method logic
        }

        public void accountSummaryEnd(int reqId)
        {
            // TODO: implement method logic
        }

        public void bondContractDetails(int reqId, ContractDetails contract)
        {
            // TODO: implement method logic
        }

        public void updateAccountValue(string key, string value, string currency, string accountName)
        {
            // TODO: implement method logic
        }

        public void updatePortfolio(Contract contract, double position, double marketPrice, double marketValue, double averageCost, double unrealizedPNL, double realizedPNL, string accountName)
        {
            // TODO: implement method logic
        }

        public void updateAccountTime(string timestamp)
        {
            // TODO: implement method logic
        }

        public void contractDetails(int reqId, ContractDetails contractDetails)
        {
            // TODO: implement method logic
        }

        public void contractDetailsEnd(int reqId)
        {
            // TODO: implement method logic
        }

        public void execDetails(int reqId, Contract contract, Execution execution)
        {
            // TODO: implement method logic
        }

        public void execDetailsEnd(int reqId)
        {
            // TODO: implement method logic
        }

        public void commissionReport(CommissionReport commissionReport)
        {
            // TODO: implement method logic
        }

        public void historicalDataUpdate(int reqId, Bar bar)
        {
            // TODO: implement method logic
        }

        public void marketDataType(int reqId, int marketDataType)
        {
            // TODO: implement method logic
        }

        public void updateMktDepth(int tickerId, int position, int operation, int side, double price, int size)
        {
            // TODO: implement method logic
        }

        public void updateMktDepthL2(int tickerId, int position, string marketMaker, int operation, int side, double price, int size)
        {
            // TODO: implement method logic
        }

        public void updateNewsBulletin(int msgId, int msgType, string message, string origExchange)
        {
            // TODO: implement method logic
        }

        public void realtimeBar(int reqId, long time, double open, double high, double low, double close, long volume, double WAP, int count)
        {
            // TODO: implement method logic
        }

        public void scannerParameters(string xml)
        {
            // TODO: implement method logic
        }

        public void scannerData(int reqId, int rank, ContractDetails contractDetails, string distance, string benchmark, string projection, string legsStr)
        {
            // TODO: implement method logic
        }

        public void scannerDataEnd(int reqId)
        {
            // TODO: implement method logic
        }

        public void receiveFA(int faDataType, string faXmlData)
        {
            // TODO: implement method logic
        }

        public void verifyMessageAPI(string apiData)
        {
            // TODO: implement method logic
        }

        public void verifyCompleted(bool isSuccessful, string errorText)
        {
            // TODO: implement method logic
        }

        public void verifyAndAuthMessageAPI(string apiData, string xyzChallenge)
        {
            // TODO: implement method logic
        }

        public void verifyAndAuthCompleted(bool isSuccessful, string errorText)
        {
            // TODO: implement method logic
        }

        public void displayGroupList(int reqId, string groups)
        {
            // TODO: implement method logic
        }

        public void displayGroupUpdated(int reqId, string contractInfo)
        {
            // TODO: implement method logic
        }

        public void positionMulti(int requestId, string account, string modelCode, Contract contract, double pos, double avgCost)
        {
            // TODO: implement method logic
        }

        public void positionMultiEnd(int requestId)
        {
            // TODO: implement method logic
        }

        public void accountUpdateMulti(int requestId, string account, string modelCode, string key, string value, string currency)
        {
            // TODO: implement method logic
        }

        public void accountUpdateMultiEnd(int requestId)
        {
            // TODO: implement method logic
        }

        public void securityDefinitionOptionParameter(int reqId, string exchange, int underlyingConId, string tradingClass, string multiplier, HashSet<string> expirations, HashSet<double> strikes)
        {
            // TODO: implement method logic
        }

        public void securityDefinitionOptionParameterEnd(int reqId)
        {
            // TODO: implement method logic
        }

        public void softDollarTiers(int reqId, SoftDollarTier[] tiers)
        {
            // TODO: implement method logic
        }

        public void familyCodes(FamilyCode[] familyCodes)
        {
            // TODO: implement method logic
        }

        public void symbolSamples(int reqId, ContractDescription[] contractDescriptions)
        {
            // TODO: implement method logic
        }

        public void mktDepthExchanges(DepthMktDataDescription[] depthMktDataDescriptions)
        {
            // TODO: implement method logic
        }

        public void tickNews(int tickerId, long timeStamp, string providerCode, string articleId, string headline, string extraData)
        {
            // TODO: implement method logic
        }

        public void smartComponents(int reqId, Dictionary<int, KeyValuePair<string, char>> theMap)
        {
            // TODO: implement method logic
        }

        public void tickReqParams(int tickerId, double minTick, string bboExchange, int snapshotPermissions)
        {
            // TODO: implement method logic
        }

        public void newsProviders(NewsProvider[] newsProviders)
        {
            // TODO: implement method logic
        }

        public void newsArticle(int requestId, int articleType, string articleText)
        {
            // TODO: implement method logic
        }

        public void historicalNews(int requestId, string time, string providerCode, string articleId, string headline)
        {
            // TODO: implement method logic
        }

        public void historicalNewsEnd(int requestId, bool hasMore)
        {
            // TODO: implement method logic
        }

        public void headTimestamp(int reqId, string headTimestamp)
        {
            // TODO: implement method logic
        }

        public void histogramData(int reqId, HistogramEntry[] data)
        {
            // TODO: implement method logic
        }

        public void rerouteMktDataReq(int reqId, int conId, string exchange)
        {
            // TODO: implement method logic
        }

        public void rerouteMktDepthReq(int reqId, int conId, string exchange)
        {
            // TODO: implement method logic
        }

        public void marketRule(int marketRuleId, PriceIncrement[] priceIncrements)
        {
            // TODO: implement method logic
        }

        public void pnl(int reqId, double dailyPnL, double unrealizedPnL, double realizedPnL)
        {
            // TODO: implement method logic
        }

        public void pnlSingle(int reqId, int pos, double dailyPnL, double unrealizedPnL, double realizedPnL, double value)
        {
            // TODO: implement method logic
        }

        public void historicalTicks(int reqId, HistoricalTick[] ticks, bool done)
        {
            // TODO: implement method logic
        }

        public void historicalTicksBidAsk(int reqId, HistoricalTickBidAsk[] ticks, bool done)
        {
            // TODO: implement method logic
        }

        public void historicalTicksLast(int reqId, HistoricalTickLast[] ticks, bool done)
        {
            // TODO: implement method logic
        }

        public void tickByTickAllLast(int reqId, int tickType, long time, double price, int size, TickAttrib attribs, string exchange, string specialConditions)
        {
            // TODO: implement method logic
        }

        public void tickByTickBidAsk(int reqId, long time, double bidPrice, double askPrice, int bidSize, int askSize, TickAttrib attribs)
        {
            // TODO: implement method logic
        }

        public void tickByTickMidPoint(int reqId, long time, double midPoint)
        {
            // TODO: implement method logic
        }

        public void tickString(int tickerId, int field, string value)
        {
            throw new NotImplementedException();
        }

        public void tickGeneric(int tickerId, int field, double value)
        {
            throw new NotImplementedException();
        }
        // ...
    }
}
