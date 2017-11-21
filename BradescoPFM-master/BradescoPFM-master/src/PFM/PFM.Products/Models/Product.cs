using PFM.Products.Interfaces;
using System.Diagnostics;

namespace PFM.Products.Models
{
    [DebuggerDisplay("{Id}, {Price}")]
    public class Product : IProduct
    {
        public string Id { get; }
        public decimal Price { get; }

        public Product(string id, decimal price)
        {
            Id = id;
            Price = price;
        }
    }
}