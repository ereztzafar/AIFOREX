using System;

namespace AIFOREX
{
    public class AIDecisionEngine
    {
        public DecisionResult Decide(DecisionInputBase input)
        {
            Console.WriteLine($"[AI DEBUG] סימול: {input.Symbol}, RSI: {input.RSI}, MACD: {input.MACD}, מחיר: {input.CurrentPrice}, MA150: {input.MA150}, MA200: {input.MA200}");

            string reason = string.Empty;

            bool isOversold = input.RSI < 45;
            bool isOverbought = input.RSI > 65;
            bool macdPositive = input.MACD > 0;
            bool macdNegative = input.MACD < 0;
            bool priceAboveMA = input.CurrentPrice > input.MA150 || input.CurrentPrice > input.MA200;

            if (isOversold && macdPositive && priceAboveMA)
            {
                reason = "📈 BUY: תנאים טכניים תומכים";
                return new DecisionResult("BUY", reason);
            }

            if (isOverbought && macdNegative)
            {
                reason = "📉 SELL: תנאים טכניים שליליים";
                return new DecisionResult("SELL", reason);
            }

            reason = "⏸ HOLD: אין איתות מובהק";
            return new DecisionResult("HOLD", reason);
        }
    }

    public class DecisionResult
    {
        public string Action { get; set; }
        public string Reason { get; set; }

        public DecisionResult(string action, string reason)
        {
            Action = action;
            Reason = reason;
        }
    }
}
