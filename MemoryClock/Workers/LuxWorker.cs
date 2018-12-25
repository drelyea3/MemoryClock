#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

using MemoryClock.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace MemoryClock.Workers
{
    public class LuxWorker : Worker
    {
        public const double INVALID_LUX = -1.0;
        LuxSensor sensor;

        public double Lux
        {
            get { return (double)GetValue(LuxProperty); }
            set { SetValue(LuxProperty, value); }
        }

        public static readonly DependencyProperty LuxProperty =
            DependencyProperty.Register("Lux", typeof(double), typeof(LuxWorker), new PropertyMetadata(INVALID_LUX));
       
        public LuxWorker()
        {
            IsRaspberryPiOnly = true;
        }

        protected override void Setup()
        {
            base.Setup();

            sensor = LuxSensor.Create(false, LuxSensor.Sensitivity.High);
        }

        protected override bool DoWork()
        {
            double lux = INVALID_LUX;

            if (sensor == null)
            {
                Update(lux);
                return false;
            }

            try
            {
                lux = sensor.GetLux();
                return true;
            }
            finally
            {
                System.Diagnostics.Debug.WriteLine($"Lux {lux}");
                Update(lux);
            }
        }

        protected override void TearDown()
        {
            base.TearDown();

            sensor?.Dispose();
        }

        private void Update(double lux)
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => Lux = lux);
        }
    }
}
