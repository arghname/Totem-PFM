using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PFM.Library.Utils
{
    public class CsvReader<TOutput>
        where TOutput : class
    {
        public static IEnumerable<TOutput> ParseCsv(string csvPath, Func<string[], TOutput> parser)
        {
            if (string.IsNullOrEmpty(csvPath))
                throw new ArgumentNullException(nameof(csvPath));
            if (!File.Exists(csvPath))
                throw new FileNotFoundException("File not found", csvPath);

            var lines = File.ReadAllLines(csvPath);
            var output = new List<TOutput>();
            foreach (var line in lines)
            {
                var fields = line.Split(',');
                output.Add(parser(fields));
            }
            return output;
        }
    }
}