#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

using System;
using Windows.UI.Xaml;

namespace MemoryClock.Workers
{
    public class BrightnessSourceWorker : Worker
    {
        public double Brightness
        {
            get { return (double)GetValue(BrightnessProperty); }
            set { SetValue(BrightnessProperty, value); }
        }

        public static readonly DependencyProperty BrightnessProperty =
            DependencyProperty.Register("Brightness", typeof(double), typeof(BrightnessSourceWorker), new PropertyMetadata(0.0));

        public DateTime Time
        {
            get { return (DateTime)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register("Time", typeof(DateTime), typeof(BrightnessSourceWorker), new PropertyMetadata(DateTime.MinValue, (o, e) => ((BrightnessSourceWorker)o).OnTimeChanged((DateTime)e.NewValue)));

        private void OnTimeChanged(DateTime newValue)
        {
            if (Lux == LuxWorker.INVALID_LUX)
            {
                var isDim = Global.Settings.IsDim.GetValue(Time.TimeOfDay);
                var brightness = isDim ? Global.Settings.NightBrightness : Global.Settings.DayBrightness;
                Update(brightness);
            }
        }

        public double Lux
        {
            get { return (double)GetValue(LuxProperty); }
            set { SetValue(LuxProperty, value); }
        }

        public static readonly DependencyProperty LuxProperty =
            DependencyProperty.Register("Lux", typeof(double), typeof(BrightnessSourceWorker), new PropertyMetadata(LuxWorker.INVALID_LUX, (o, e) => ((BrightnessSourceWorker)o).OnLuxChanged((double)e.NewValue)));

        private void OnLuxChanged(double newValue)
        {
            if (newValue != LuxWorker.INVALID_LUX)
            { 
                double MinOpacity = Global.Settings.NightBrightness;
                var newBrightness = (1.0 - MinOpacity) * newValue + MinOpacity;
                Update(newBrightness);
            }
        }

        protected override bool DoWork()
        {
            return true;
        }

        private void Update(double newBrightness)
        {
            //Debug.WriteLine($"Brightness {newBrightness}");
            Brightness = newBrightness;
        }
    }
}
