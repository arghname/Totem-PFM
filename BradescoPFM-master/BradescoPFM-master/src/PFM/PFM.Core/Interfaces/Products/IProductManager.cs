using PFM.Core.Models.Products;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PFM.Core.Interfaces.Products
{
    public interface IProductManager
    {
        Task<IEnumerable<IProduct>> GetAsync();

        Task<IProduct> GetAsync(string id);

        Task InsertAsync(IProduct product);
    }
}
