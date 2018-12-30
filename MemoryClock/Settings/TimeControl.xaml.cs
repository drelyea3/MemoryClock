using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MemoryClock.Settings
{
    public sealed partial class TimeControl : UserControl, ISettingsPage
    {
        TimeSpan[] hoursData;
        TimeSpan[] minutesData;

        public TimeControl()
        {
            this.InitializeComponent();
        }

        public void Cancel()
        {
        }

        public void Hide()
        {
            this.Visibility = Visibility.Collapsed;
        }

        public void Save()
        {
        }

        public void Show()
        {
            this.Visibility = Visibility.Visible;
            if (hoursData == null)
            {
                hoursData = new TimeSpan[24];
                for (int h = 0; h < 24; ++h)
                {
                    hoursData[h] = TimeSpan.FromHours(h);
                }
                hours.ItemsSource = hoursData;

                minutesData = new TimeSpan[60];
                for (int m = 0; m < 60; ++m)
                {
                    minutesData[m] = TimeSpan.FromMinutes(m);
                }
                minutes.ItemsSource = minutesData;
            }

            var timeWorker = App.Current.Resources["TimeSource"] as Workers.TimeSourceWorker;
            var now = timeWorker.Now;

            hours.SelectedIndex = now.Hour;
            minutes.SelectedIndex= now.Minute;
        }

        private void OnTimeSet(object sender, RoutedEventArgs e)
        {
            var timeWorker = App.Current.Resources["TimeSource"] as Workers.TimeSourceWorker;
            var system = timeWorker.Now.TimeOfDay;
            var manual = (TimeSpan)hours.SelectedItem + (TimeSpan)minutes.SelectedItem;
            var drift = manual - system;
            timeWorker.Adjustment += drift;
            
        }
    }
}
