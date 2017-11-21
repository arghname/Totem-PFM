using PFM.Core.Interfaces.Products;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PFM.Core.Models.Products
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
