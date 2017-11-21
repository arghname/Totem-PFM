using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PFM.Bot.Models;
using System.Text;
using System.Globalization;

namespace PFM.Bot.Utils
{
    public class Summarizer
    {
        private CultureInfo _cultureInfo;
        private StringBuilder _builder;

        public Summarizer()
        {
            _cultureInfo = CultureInfo.GetCultureInfo("pt-BR");
            _builder = new StringBuilder();
        }

        public string GetSummary(IEnumerable<SummaryEntry> weeklyEntries, string inMessage, string outMessage)
        {
            _builder.Clear();

            var income = weeklyEntries.Where(e => e.Category == "In");
            ProduceSummary(_builder, inMessage, income);
            var outcome = weeklyEntries.Where(e => e.Category == "Out");
            ProduceSummary(_builder, outMessage, outcome);

            return _builder.ToString();
        }

        private void ProduceSummary(StringBuilder builder, string title, IEnumerable<SummaryEntry> entries)
        {
            builder.AppendLine(title);
            foreach (var entry in entries)
            {
                builder.AppendLine($"- {entry.Name} no valor de {entry.Value.ToString("C", _cultureInfo)}");
            }
            builder.AppendLine();
        }
    }
}