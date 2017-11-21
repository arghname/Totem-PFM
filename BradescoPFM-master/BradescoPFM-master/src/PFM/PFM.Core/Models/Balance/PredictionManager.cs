using Newtonsoft.Json;
using PFM.Core.Interfaces.Balance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PFM.Core.Models.Balance
{
    public class PredictionManager: IPredictionManager, IDisposable
    {
        public string BalanceEndpoint { get; }
        private HttpClient _httpClient;

        public PredictionManager(string balanceEndpoint)
        {
            BalanceEndpoint = balanceEndpoint;
            _httpClient = new HttpClient();
        }

        public async Task<PredictionRecommendation> GetAvailableMonthForPurchaseAsync(string accountId, decimal value)
        {
            var url = string.Concat(BalanceEndpoint, "/api/predict/purchase/", accountId, "?", "value=", value);
            var balance = await _httpClient.GetStringAsync(url);
            return JsonConvert.DeserializeObject<PredictionRecommendation>(balance);
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
