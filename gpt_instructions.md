# 🤖 GPT Instructions – AIFOREX

## מטרת המערכת
AIFOREX מנתחת נתוני שוק (פורקס ומניות), מחשבת אינדיקטורים טכניים, שולחת המלצות בינה מלאכותית, ומבצעת מסחר אוטומטי דרך IBKR.

## חוקי עדכון GPT
- כל שינוי יתבצע בענף `gpt-update`
- אין לשנות את הקוד ב־main ישירות
- כל שינוי יכלול:
  - בדיקת Compile
  - בדיקת הרצה בסיסית
  - Pull Request בלבד, לא Merge אוטומטי

## אזורים עיקריים בקוד
- ניתוח נתונים: `StockDataFetcher.cs`, `TechnicalIndicators.cs`
- מנוע החלטות: `AIDecisionEngine.cs`, `gpt_bot.py`
- ממשקי פעולה: `TelegramNotifier.cs`, `ExcelLogger.cs`, `TradeManager.cs`

## משימות לדוגמה
- שיפור חישובי RSI על בסיס ATR
- שילוב תגובות לחדשות כלכליות (Yahoo, Bloomberg)
- קביעת Stop Loss חכם לפי VIX וניתוח טכני
