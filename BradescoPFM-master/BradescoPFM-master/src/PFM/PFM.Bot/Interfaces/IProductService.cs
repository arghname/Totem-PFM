using PFM.Bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PFM.Bot.Interfaces
{
    public interface IProductService
    {
        Task<Product> GetAsync(string id);
        Task InsertAsync(Product product);
    }
}