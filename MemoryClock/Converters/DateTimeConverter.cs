using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryClock.Converters
{
    public class DateTimeConverter : OneWayConverter<DateTime, string>
    {
        protected override object Convert(DateTime value, string format)
        {
            return value.ToString(format);
        }
    }
}
