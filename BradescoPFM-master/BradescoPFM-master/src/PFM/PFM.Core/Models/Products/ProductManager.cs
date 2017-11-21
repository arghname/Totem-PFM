using Newtonsoft.Json;
using PFM.Core.Interfaces.Products;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PFM.Core.Models.Products
{
    public class ProductManager : IProductManager, IDisposable
    {
        public string ProductsEndpoint { get; }
        private HttpClient _httpClient;

        public ProductManager(string productsEndpoint)
        {
            ProductsEndpoint = productsEndpoint;
            _httpClient = new HttpClient();
        }

        public async Task<IEnumerable<IProduct>> GetAsync()
        {
            var url = string.Concat(ProductsEndpoint, "/api/products/");
            var products = await _httpClient.GetStringAsync(url);
            return JsonConvert.DeserializeObject<IEnumerable<Product>>(products);
        }

        public async Task<IProduct> GetAsync(string id)
        {
            var url = string.Concat(ProductsEndpoint, "/api/products/", id);
            var product = await _httpClient.GetStringAsync(url);
            return JsonConvert.DeserializeObject<Product>(product);
        }

        public async Task InsertAsync(IProduct product)
        {
            var url = string.Concat(ProductsEndpoint, "/api/products/");
            var body = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, body);
            response.EnsureSuccessStatusCode();
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
