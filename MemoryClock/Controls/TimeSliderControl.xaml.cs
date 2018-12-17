using MemoryClock.Sensors;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace MemoryClock.Controls
{
    public sealed partial class TimeSliderControl : UserControl
    {
        public TimeController TimeController
        {
            get { return (TimeController)GetValue(TimeControllerProperty); }
            set { SetValue(TimeControllerProperty, value); }
        }

        public static readonly DependencyProperty TimeControllerProperty =
            DependencyProperty.Register("TimeController", typeof(TimeController), typeof(TimeSliderControl), new PropertyMetadata(null));

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(TimeSliderControl), new PropertyMetadata(false, (o,e) => ((TimeSliderControl)o).OnIsActiveChanged((bool)e.NewValue)));

        private void OnIsActiveChanged(bool newValue)
        {
            if (TimeController != null)
            {
                TimeController.IsPaused = newValue;
            }

            timeSlider.IsEnabled = newValue;
        }

        public TimeSliderControl()
        {
            this.InitializeComponent();
        }

        private void OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (TimeController != null)
            {
                var now = DateTime.Now;
                var time = TimeSpan.FromMinutes(e.NewValue);

                var dateTime = new DateTime(now.Year, now.Month, now.Day, time.Hours, time.Minutes, time.Seconds);

                TimeController.Update(dateTime);
            }
        }
    }
}
