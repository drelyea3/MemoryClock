using System;

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
