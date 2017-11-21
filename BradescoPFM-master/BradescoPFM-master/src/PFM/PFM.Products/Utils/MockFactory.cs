using PFM.Products.Interfaces;
using PFM.Products.Models;

namespace PFM.Products.Utils
{
    public class MockFactory
    {
        public static IProduct BuildProduct(string[] fields) 
            => new Product(fields[0], decimal.Parse(fields[1]));
    }
}
