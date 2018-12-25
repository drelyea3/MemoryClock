#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

using Common;
using MemoryClock.Sensors;
using System;
using System.Diagnostics;
using Windows.UI.Xaml;

namespace MemoryClock.Workers
{
    public class BrightnessSourceWorker : Worker
    {
        private RollingAverage luxBuffer;

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
            DependencyProperty.Register("Time", typeof(DateTime), typeof(BrightnessSourceWorker), new PropertyMetadata(DateTime.MinValue, (o,e) => ((BrightnessSourceWorker)o).OnTimeChanged((DateTime)e.NewValue)));

        private void OnTimeChanged(DateTime newValue)
        {
            if (Lux == LuxWorker.INVALID_LUX)
            {
                var isDim = Global.Settings.IsDim.GetValue(Time.TimeOfDay);
                var brightness = isDim ? Global.Settings.NightBrightness : Global.Settings.DayBrightness;
                Update(brightness, LuxWorker.INVALID_LUX);
            }
        }

        public double Lux
        {
            get { return (double)GetValue(LuxProperty); }
            set { SetValue(LuxProperty, value); }
        }

        public static readonly DependencyProperty LuxProperty =
            DependencyProperty.Register("Lux", typeof(double), typeof(BrightnessSourceWorker), new PropertyMetadata(0.0, (o,e) => ((BrightnessSourceWorker)o).OnLuxChanged((double)e.OldValue, (double)e.NewValue)));

        private void OnLuxChanged(double oldValue, double newValue)
        {
            if (newValue == LuxWorker.INVALID_LUX)
            {
                luxBuffer = null;
            }
            else
            {
                if (luxBuffer == null)
                {
                     luxBuffer = new RollingAverage(5);
                }

                double MaxLux = Global.Settings.MaxLux;
                double MinOpacity = Global.Settings.NightBrightness;

                var normalizedLux = Math.Min(1.0, Lux / MaxLux);

                luxBuffer.Update(normalizedLux);
                var averageLux = luxBuffer.GetAverage();

                var newBrightness = (1.0 - MinOpacity) * averageLux + MinOpacity;
                Update(newBrightness, Lux);
            }
        }

        protected override bool DoWork()
        {
            return true;
        }

        private void Update(double newBrightness, double currentLux)
        {
            Brightness = newBrightness;
            Lux = currentLux;
        }
    }
}
