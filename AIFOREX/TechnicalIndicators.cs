using System;
using System.Collections.Generic;
using System.Linq;

namespace AIFOREX
{
    public static class TechnicalIndicators
    {
        public static List<double> CalculateSMA(List<double> prices, int period)
        {
            var sma = new List<double>();
            if (prices.Count < period)
                return sma;

            for (int i = 0; i <= prices.Count - period; i++)
                sma.Add(prices.Skip(i).Take(period).Average());
            return sma;
        }

        public static List<double> CalculateRSI(List<double> prices, int period)
        {
            var rsi = new List<double>();
            if (prices.Count <= period)
                return rsi;

            for (int i = period; i < prices.Count - 1; i++)
            {
                double gain = 0, loss = 0;
                for (int j = i - period + 1; j <= i; j++)
                {
                    double diff = prices[j + 1] - prices[j];
                    if (diff >= 0) gain += diff;
                    else loss -= diff;
                }

                if (loss == 0) rsi.Add(100);
                else
                {
                    double rs = gain / loss;
                    rsi.Add(100 - (100 / (1 + rs)));
                }
            }
            return rsi;
        }

        public static (List<double> macd, List<double> signal) CalculateMACD(List<double> prices)
        {
            if (prices.Count < 26)
                return (new List<double>(), new List<double>());

            var ema12 = CalculateEMA(prices, 12);
            var ema26 = CalculateEMA(prices, 26);
            var macd = ema12.Zip(ema26, (e12, e26) => e12 - e26).ToList();
            var signal = CalculateEMA(macd, 9);
            return (macd, signal);
        }

        public static List<double> CalculateEMA(List<double> prices, int period)
        {
            var ema = new List<double>();
            if (prices.Count < period)
                return ema;

            double multiplier = 2.0 / (period + 1);
            ema.Add(prices.Take(period).Average());
            for (int i = period; i < prices.Count; i++)
                ema.Add((prices[i] - ema[^1]) * multiplier + ema[^1]);
            return ema;
        }
    }
}
