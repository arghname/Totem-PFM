using System.Collections.Generic;

namespace PFM.Products.Interfaces
{
    public interface IProductManager
    {
        IEnumerable<IProduct> Get();
        IProduct Get(string id);
        void Insert(IProduct product);
    }
}
