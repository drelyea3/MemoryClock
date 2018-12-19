using System;

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
