using PFM.Bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFM.Bot.Interfaces
{
    public interface ICoreService
    {
        //Alerts
        Task<Alert> CheckNegativeBalance(string accountId);

        //Prediction
        Task<PredictionRecommendation> GetAvailableMonthForPurchase(string accountId, string productId);

        //Summary
        Task<Summary> LastWeekSummary(string accountId);
        Task<Summary> NextWeekSummary(string accountId);
    }
}
