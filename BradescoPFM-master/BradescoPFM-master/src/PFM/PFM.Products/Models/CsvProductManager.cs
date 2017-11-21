using PFM.Products.Interfaces;
using PFM.Products.Utils;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PFM.Products.Models
{
    public class CsvProductManager : IProductManager
    {
        private IEnumerable<IProduct> _products;

        public CsvProductManager()
        {
            var csvFile = Path.Combine(Directory.GetCurrentDirectory(), @"Data\products.csv");
            _products = CsvReader<IProduct>.ParseCsv(csvFile, MockFactory.BuildProduct);
        }

        public IEnumerable<IProduct> Get() => _products;

        public IProduct Get(string id) => _products.FirstOrDefault(p => p.Id == id.ToUpper());

        public void Insert(IProduct product) => ((IList)_products).Add(product);
    }
}