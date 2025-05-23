namespace AIFOREX
{
    public record DecisionInputBase(
        string Symbol,
        decimal RSI,
        decimal MACD,
        decimal Signal,
        decimal CurrentPrice,
        decimal VIX,
        decimal MA50,
        decimal MA150,
        decimal MA200
    )
    {
        // קונסטרקטור נוסף שתומך ב-double, ממיר ל-decimal
        public DecisionInputBase(string symbol, double rsi, double macd, double signal, double currentPrice, double vix, double ma50, double ma150, double ma200)
            : this(
                symbol,
                (decimal)rsi,
                (decimal)macd,
                (decimal)signal,
                (decimal)currentPrice,
                (decimal)vix,
                (decimal)ma50,
                (decimal)ma150,
                (decimal)ma200)
        {
        }
    }
}
