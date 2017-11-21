using PFM.Bot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PFM.Bot.Models;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Configuration;
using PFM.Bot.Utils;

namespace PFM.Bot.Services
{
    public class ProductService : IProductService
    {
        private string ProductsEndpoint = ConfigurationManager.AppSettings["ProductsEndpoint"];

        public async Task<Product> GetAsync(string id)
        {
            var url = string.Concat(ProductsEndpoint, "/api/products/", id);
            return await CallService<Product>(url);
        }

        public async Task InsertAsync(Product product)
        {
            var url = string.Concat(ProductsEndpoint, "/api/products/");
            await PostService(url, product);
        }

        private async Task<T> CallService<T>(string url)
        {
            using (var httpClient = new HttpClient())
            {
                var balance = await httpClient.GetStringAsync(url);
                return JsonConvert.DeserializeObject<T>(balance);
            }
        }

        private async Task PostService(string url, object content)
        {
            using (var httpClient = new HttpClient())
            {
                var body = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(url, body);
                response.EnsureSuccessStatusCode();
            }
        }
    }
}