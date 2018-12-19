using System;
using Windows.UI.Xaml;

namespace MemoryClock.Converters
{
    class NoPhoneConverter : OneWayConverter<DateTime>
    {
        protected override object Convert(DateTime value)
        {
            return Global.Settings.IsNoPhone.GetValue(value.TimeOfDay) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
