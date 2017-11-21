using System;
using System.Diagnostics;

namespace PFM.Bot.Models
{
    [DebuggerDisplay("{Id}, {Price}")]
    [Serializable]
    public class Product
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