using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MemoryClock.Converters
{
    public abstract class OneWayConverter<T> : IValueConverter
    {
        protected abstract object Convert(T value);

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Convert((T)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class OneWayConverter<T, P> : IValueConverter
    {
        protected abstract object Convert(T value, P parameter);

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Convert((T)value, (P)parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
