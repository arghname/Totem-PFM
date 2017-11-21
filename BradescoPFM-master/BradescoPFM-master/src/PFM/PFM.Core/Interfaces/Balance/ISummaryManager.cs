using PFM.Core.Models.Balance;
using System.Threading.Tasks;

namespace PFM.Core.Interfaces.Balance
{
    public interface ISummaryManager
    {
        Task<Summary> GetPastSummaryAsync(string accountId, int days);
        Task<Summary> GetFutureSummaryAsync(string accountId, int days);
        Task<byte[]> GetChartAsync(string accountId, int days);
    }
}
