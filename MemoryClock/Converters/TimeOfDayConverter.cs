using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryClock.Converters
{
    class TimeOfDayConverter : OneWayConverter<DateTime>
    {
        protected override object Convert(DateTime value)
        {
            return Global.Settings.Message.GetValue(value.TimeOfDay);
        }
    }
}
