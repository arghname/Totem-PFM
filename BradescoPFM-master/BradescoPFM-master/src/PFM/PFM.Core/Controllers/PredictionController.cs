using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PFM.Core.Models.Products;
using PFM.Core.Interfaces.Products;
using PFM.Core.Interfaces.Balance;
using PFM.Core.Models.Balance;

namespace PFM.Core.Controllers
{
    [Route("api/[controller]")]
    public class PredictionController : Controller
    {
        private readonly IProductManager _productManager;
        private readonly IPredictionManager _predictionManager;

        public PredictionController(IProductManager productManager, IPredictionManager predictionManager)
        {
            _productManager = productManager;
            _predictionManager = predictionManager;
        }

        [HttpGet("GetAvailableMonthForPurchase")]
        public async Task<PredictionRecommendation> GetAvailableMonthForPurchase([FromQuery]string accountId, [FromQuery]string productId)
        {
            var product = await _productManager.GetAsync(productId);
            var value = product.Price;
            var availableMonth = await _predictionManager.GetAvailableMonthForPurchaseAsync(accountId, value);
            return availableMonth;
        }
    }
}
