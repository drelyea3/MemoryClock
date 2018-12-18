using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryClock.Converters
{
    public class TimeSpanToTimeConverter : OneWayConverter<TimeSpan, string>
    {
        protected override object Convert(TimeSpan value, string format)
        {
            var dt = new DateTime(2018, 12, 18, value.Hours, value.Minutes, value.Seconds);
            return dt.ToString(format);
        }
    }
}
