using Newtonsoft.Json;
using PFM.Core.Interfaces.Balance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PFM.Core.Models.Balance
{
    public class BalanceManager : IBalanceManager, IDisposable
    {
        public string BalanceEndpoint { get; }
        private HttpClient _httpClient;

        public BalanceManager(string balanceEndpoint)
        {
            BalanceEndpoint = balanceEndpoint;
            _httpClient = new HttpClient();
        }

        public async Task<BalanceData> GetBalanceAsync(string accountId)
        {
            var url = string.Concat(BalanceEndpoint, "/api/balance/", accountId);
            var balance = await _httpClient.GetStringAsync(url);
            return JsonConvert.DeserializeObject<BalanceData>(balance);
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
