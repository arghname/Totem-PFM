using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PFM.Bot.Models
{
    public class PredictionRecommendation
    {
        public string AccountId { get; set; }
        public decimal Value { get; set; }
        public int AvailableMonth { get; set; }
    }
}
