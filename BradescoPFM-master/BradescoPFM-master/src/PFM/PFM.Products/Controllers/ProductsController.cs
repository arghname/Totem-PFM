using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PFM.Products.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using PFM.Products.Models;

namespace PFM.Products.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private readonly IProductManager _productManager;

        public ProductsController(IProductManager productManager)
        {
            _productManager = productManager;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<IProduct> Get() => _productManager.Get();

        // GET api/values/5
        [HttpGet("{id}")]
        public IProduct Get(string id) => _productManager.Get(id);

        // POST api/values
        [HttpPost]
        public void Post([FromBody]Product product) => _productManager.Insert(product);
    }
}
