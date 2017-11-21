using PFM.Bot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PFM.Bot.Models;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Http;
using Newtonsoft.Json;

namespace PFM.Bot.Services
{
    public class CoreService : ICoreService
    {
        private string CoreEndpoint = ConfigurationManager.AppSettings["CoreEndpoint"];

        public async Task<Alert> CheckNegativeBalance(string accountId)
        {
            var url = string.Concat(CoreEndpoint, "/api/alerts/CheckNegativeBalance", "?", "accountId=", accountId);
            return await CallService<Alert>(url);
        }

        public async Task<PredictionRecommendation> GetAvailableMonthForPurchase(string accountId, string productId)
        {
            var url = string.Concat(CoreEndpoint, "/api/prediction/GetAvailableMonthForPurchase", "?", "accountId=", accountId, "&", "productId=", productId);
            return await CallService<PredictionRecommendation>(url);
        }

        public async Task<Summary> LastWeekSummary(string accountId)
        {
            var url = string.Concat(CoreEndpoint, "/api/summary/LastWeekSummary", "?", "accountId=", accountId);
            return await CallService<Summary>(url);
        }

        public async Task<Summary> NextWeekSummary(string accountId)
        {
            var url = string.Concat(CoreEndpoint, "/api/summary/NextWeekSummary", "?", "accountId=", accountId);
            return await CallService<Summary>(url);
        }

        private async Task<T> CallService<T>(string url)
        {
            using (var httpClient = new HttpClient())
            {
                var balance = await httpClient.GetStringAsync(url);
                return JsonConvert.DeserializeObject<T>(balance);
            }
        }
    }
}