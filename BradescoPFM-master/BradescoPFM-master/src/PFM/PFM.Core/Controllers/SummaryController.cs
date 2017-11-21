using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PFM.Core.Interfaces.Balance;
using PFM.Core.Models.Balance;

namespace PFM.Core.Controllers
{
    [Route("api/[controller]")]
    public class SummaryController : Controller
    {
        private readonly ISummaryManager _summaryManager;

        public SummaryController(ISummaryManager summaryManager)
        {
            _summaryManager = summaryManager;
        }

        [HttpGet("LastWeekSummary")]
        public async Task<Summary> LastWeekSummary([FromQuery]string accountId)
        {
            return await _summaryManager.GetPastSummaryAsync(accountId, days: 7);
        }

        [HttpGet("NextWeekSummary")]
        public async Task<Summary> NextWeekSummary([FromQuery]string accountId)
        {
            return await _summaryManager.GetFutureSummaryAsync(accountId, days: 7);
        }
    }
}
