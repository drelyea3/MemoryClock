using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
