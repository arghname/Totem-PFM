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
    public class AlertsController : Controller
    {
        private readonly IBalanceManager _balanceManager;

        public AlertsController(IBalanceManager balanceManager)
        {
            _balanceManager = balanceManager;
        }

        // GET api/alerts/CheckNegativeBalance?accountId=abc123
        [HttpGet("CheckNegativeBalance")]
        public async Task<Alert> CheckNegativeBalance([FromQuery]string accountId)
        {
            var balance = await _balanceManager.GetBalanceAsync(accountId);
            if (balance.Value < 0)
                return new Alert("Sua conta está negativa!", "Critical");
            else if (balance.Value > 0)
                return new Alert("Sua conta está saudável!", "Warning");
            else
                return new Alert("Sua conta está zerada!", "Default");
        }

    }
}
