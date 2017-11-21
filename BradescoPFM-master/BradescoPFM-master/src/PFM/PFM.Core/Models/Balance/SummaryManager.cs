using Newtonsoft.Json;
using PFM.Core.Interfaces.Balance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PFM.Core.Models.Balance
{
    public class SummaryManager : ISummaryManager, IDisposable
    {
        public string BalanceEndpoint { get; }
        private HttpClient _httpClient;

        public SummaryManager(string balanceEndpoint)
        {
            BalanceEndpoint = balanceEndpoint;
            _httpClient = new HttpClient();
        }

        public async Task<Summary> GetPastSummaryAsync(string accountId, int days)
        {
            var url = string.Concat(BalanceEndpoint, "/api/summary/past/", accountId, "?", "days=", days);
            var balance = await _httpClient.GetStringAsync(url);
            return JsonConvert.DeserializeObject<Summary>(balance);
        }

        public async Task<Summary> GetFutureSummaryAsync(string accountId, int days)
        {
            var url = string.Concat(BalanceEndpoint, "/api/summary/future/", accountId, "?", "days=", days);
            var balance = await _httpClient.GetStringAsync(url);
            return JsonConvert.DeserializeObject<Summary>(balance);
        }

        public Task<byte[]> GetChartAsync(string accountId, int days)
        {
            throw new NotImplementedException();
        }

        #region Disposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                if (_httpClient != null)
                    _httpClient.Dispose();
        }

        #endregion
    }
}
