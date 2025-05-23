using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AIFOREX.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private static readonly List<DashboardEntry> _log = new();

        // POST: קבל נתון חדש לדשבורד
        [HttpPost("log")]
        public IActionResult LogEntry([FromBody] DashboardEntry entry)
        {
            _log.Add(entry);
            if (_log.Count > 100)
                _log.RemoveAt(0); // שמור עד 100 רשומות

            return Ok();
        }

        // GET: שלוף את כל הנתונים לתצוגה
        [HttpGet("data")]
        public ActionResult<IEnumerable<DashboardEntry>> GetAll()
        {
            return Ok(_log);
        }
    }

    public class DashboardEntry
    {
        public string Time { get; set; }
        public string Symbol { get; set; }
        public string Action { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public double PnL { get; set; }
        public string Source { get; set; } // מי שלח: AI / PortfolioChecker / TradeManager
    }
}
