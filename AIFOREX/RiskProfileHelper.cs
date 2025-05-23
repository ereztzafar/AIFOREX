namespace AIFOREX
{
    public static class RiskProfileHelper
    {
        /// <summary>
        /// מחשב את גורם הסיכון לנכס. ערך גבוה = סיכון גבוה יותר → השקעה קטנה יותר.
        /// </summary>
        public static double GetRiskFactor(string category, string symbol)
        {
            category = category.ToUpper();
            symbol = symbol.ToUpper();

            switch (category)
            {
                case "SP500":
                    if (symbol == "TSLA" || symbol == "NVDA")
                        return 1.5; // מניות תנודתיות
                    if (symbol == "AAPL" || symbol == "MSFT")
                        return 0.8; // יציבות יחסית
                    return 1.0;

                case "FOREX":
                    if (symbol == "USDJPY" || symbol == "GBPJPY")
                        return 1.3; // צמדים תנודתיים
                    if (symbol == "EURUSD")
                        return 1.0;
                    return 1.2;

                case "TASE":
                    if (symbol == "פועלים" || symbol == "לאומי")
                        return 0.9; // מניות בנק יציבות
                    if (symbol == "טבע")
                        return 1.4; // תנודתית יותר
                    return 1.1;

                default:
                    return 1.0; // ברירת מחדל
            }
        }
    }
}

