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
        public static void Log(string message)
        {
            var lines = message.Split(Environment.NewLine);
            var stamp = DateTime.Now.ToString("s");
            foreach (var line in lines)
            { 
                Debug.WriteLine($"{stamp} - {line}");
            }
        }
    }
}
