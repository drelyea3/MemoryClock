using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class Logger
    {
        public static List<Tuple<DateTime, string>> buffer = new List<Tuple<DateTime, string>>();

        public static void Log(string message)
        {
            var lines = message.Split(Environment.NewLine);
            var now = DateTime.Now;
            var stamp = now.ToString("s");
            //buffer.Add(new Tuple<DateTime, string>(now, message));

            foreach (var line in lines)
            { 
                Debug.WriteLine($"{stamp} - {line}");
            }
        }
    }
}
