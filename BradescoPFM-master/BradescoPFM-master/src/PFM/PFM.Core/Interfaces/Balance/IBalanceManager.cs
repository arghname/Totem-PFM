using PFM.Core.Models.Balance;
using System.Threading.Tasks;

namespace PFM.Core.Interfaces.Balance
{
    public interface IBalanceManager
    {
        Task<BalanceData> GetBalanceAsync(string accountId);
    }
}
