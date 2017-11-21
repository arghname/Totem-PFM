using PFM.Core.Models.Balance;
using System;
using System.Threading.Tasks;

namespace PFM.Core.Interfaces.Balance
{
    public interface IPredictionManager
    {
        Task<PredictionRecommendation> GetAvailableMonthForPurchaseAsync(string accountId, decimal value);
    }
}
