using MemoryClock.Sensors;
using MemoryClock.Workers;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace MemoryClock.Controls
{
    public sealed partial class TimeSliderControl : UserControl
    {
        public TimeSourceWorker TimeSource
        {
            get { return (TimeSourceWorker)GetValue(TimeSourceProperty); }
            set { SetValue(TimeSourceProperty, value); }
        }

        public static readonly DependencyProperty TimeSourceProperty =
            DependencyProperty.Register("TimeSource", typeof(TimeSourceWorker), typeof(TimeSliderControl), new PropertyMetadata(null));

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(TimeSliderControl), new PropertyMetadata(false, (o,e) => ((TimeSliderControl)o).OnIsActiveChanged((bool)e.NewValue)));

        private void OnIsActiveChanged(bool newValue)
        {
            if (TimeSource != null)
            {
                if (newValue)
                {
                    TimeSource.Stop();
                }
                else
                {
                    TimeSource.Start();
                }
            }

            timeSlider.IsEnabled = newValue;
        }

        public TimeSliderControl()
        {
            this.InitializeComponent();
        }

        private void OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (TimeSource != null)
            {
                var now = DateTime.Now;
                var time = TimeSpan.FromMinutes(e.NewValue);

                var dateTime = new DateTime(now.Year, now.Month, now.Day, time.Hours, time.Minutes, time.Seconds);

                TimeSource.Update(dateTime);
            }
        }
    }
}
