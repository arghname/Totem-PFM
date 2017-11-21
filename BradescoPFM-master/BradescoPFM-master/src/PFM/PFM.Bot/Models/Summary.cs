using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PFM.Bot.Models
{
    public class Summary
    {
        public string AccountId { get; set; }
        public int Days { get; set; }
        public IEnumerable<SummaryEntry> Entries { get; set; }
    }
}
