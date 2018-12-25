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
        public enum SensorState
        {
            OK,
            NotAvailable,
            Malfunctioning,
        }

        public TimeSourceWorker TimeSource
        {
            get { return (TimeSourceWorker)GetValue(TimeSourceProperty); }
            set { SetValue(TimeSourceProperty, value); }
        }

        public static readonly DependencyProperty TimeSourceProperty =
            DependencyProperty.Register("TimeSource", typeof(TimeSourceWorker), typeof(BrightnessSourceWorker), new PropertyMetadata(null));

        private RollingAverage luxBuffer = new RollingAverage(5);

        public SensorState State { get; private set; } = SensorState.OK;

        LuxSensor sensor;

        public double Brightness
        {
            get { return (double)GetValue(BrightnessProperty); }
            set { SetValue(BrightnessProperty, value); }
        }

        public static readonly DependencyProperty BrightnessProperty =
            DependencyProperty.Register("Brightness", typeof(double), typeof(BrightnessSourceWorker), new PropertyMetadata(0.0));

        public double Lux
        {
            get { return (double)GetValue(LuxProperty); }
            set { SetValue(LuxProperty, value); }
        }

        public static readonly DependencyProperty LuxProperty =
            DependencyProperty.Register("Lux", typeof(double), typeof(BrightnessSourceWorker), new PropertyMetadata(0));

        protected override bool DoWork()
        {
            if (State == SensorState.OK)
            {
                if (sensor == null)
                {
                    sensor = LuxSensor.Create();
                    if (sensor == null)
                    {
                        Logger.Log($"{this.GetType()} Could not create sensor, falling back to schedule");
                        State = SensorState.NotAvailable;
                        Fallback();
                        return false;
                    }
                }

                double currentLux = sensor.GetLux();
                double MaxLux = Global.Settings.MaxLux;
                double MinOpacity = Global.Settings.NightBrightness;

                var normalizedLux = Math.Min(1.0, currentLux / MaxLux);

                luxBuffer.Update(normalizedLux);
                var averageLux = luxBuffer.GetAverage();

                var newBrightness = (1.0 - MinOpacity) * averageLux + MinOpacity;

                Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    Update(newBrightness, currentLux);
                });

                return true;
            }

            sensor?.Dispose();
            sensor = null;

            Fallback();           
            return false;
        }

        private void Update(double newBrightness, double currentLux)
        {
            Brightness = newBrightness;
            Lux = currentLux;
        }

        private void Fallback()
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (TimeSource != null)
                {
                    TimeSource.Tick += (o, e) =>
                    {
                        var isDim = Global.Settings.IsDim.GetValue(TimeSource.Now.TimeOfDay);
                        var brightness = isDim ? Global.Settings.NightBrightness : Global.Settings.DayBrightness;
                        Update(brightness, 0);
                    };
                }
            });
        }
    }
}
