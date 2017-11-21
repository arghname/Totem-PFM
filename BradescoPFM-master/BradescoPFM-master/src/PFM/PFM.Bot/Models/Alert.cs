using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PFM.Bot.Models
{
    public class Alert
    {
        public string Message { get; }
        public string Level { get; }

        public Alert(string message, string level)
        {
            Message = message;
            Level = level;
        }
    }
}
