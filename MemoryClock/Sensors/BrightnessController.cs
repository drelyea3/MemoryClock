#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System.Threading;
using Windows.UI.Xaml;

namespace MemoryClock.Sensors
{
    public class BrightnessController : DependencyObject
    {
        public enum SensorState
        {
            OK,
            NotAvailable,
            Malfunctioning,
        }

        private RollingAverage luxBuffer = new RollingAverage(5);

        public SensorState State { get; private set; }

        public double Brightness
        {
            get { return (double)GetValue(BrightnessProperty); }
            set { SetValue(BrightnessProperty, value); }
        }

        public static readonly DependencyProperty BrightnessProperty =
            DependencyProperty.Register("Brightness", typeof(double), typeof(BrightnessController), new PropertyMetadata(1.0));

        public BrightnessController()
        {
            ThreadPool.RunAsync(LuxWorker);
        }

        private void LuxWorker(IAsyncAction operation)
        {
            var sensor = LuxSensor.Create();
            var sensorOK = true;
            if (sensor != null)
            {
                while (sensorOK)
                {
                    double currentLux = sensor.GetLux();
                    double MaxLux = Global.Settings.MaxLux;
                    double MinOpacity = Global.Settings.NightBrightness;

                    var normalizedLux = Math.Min(1.0, currentLux / MaxLux);

                    luxBuffer.Update(normalizedLux);
                    var averageLux = luxBuffer.GetAverage();

                    var newBrightness = (1.0 - MinOpacity) * averageLux + MinOpacity;

                    Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        Brightness = newBrightness;
                    });
                    Task.Delay((int)Global.Settings.LightSensorInterval.TotalMilliseconds).Wait();
                }
            }
        }
    }
}
