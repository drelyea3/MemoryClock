#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;

namespace MemoryClock.Workers
{
    public class GpioWorker: DependencyObject, IWorker
    {
        public class ValueChangedEventArgs
        {
            public ValueChangedEventArgs(bool value)
            {
                Value = value;
            }

            public bool Value { get; private set;}
        }

        private static readonly ValueChangedEventArgs ValueTrue = new ValueChangedEventArgs(true);
        private static readonly ValueChangedEventArgs ValueFalse = new ValueChangedEventArgs(false);

        public bool Value
        {
            get { return (bool)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(bool), typeof(GpioWorker), new PropertyMetadata(false));

        public int Pin
        {
            get { return (int)GetValue(PinProperty); }
            set { SetValue(PinProperty, value); }
        }

        public static readonly DependencyProperty PinProperty =
            DependencyProperty.Register("Pin", typeof(int), typeof(GpioWorker), new PropertyMetadata(0));

        public TimeSpan DebounceTimeout
        {
            get { return (TimeSpan)GetValue(DebounceTimeoutProperty); }
            set { SetValue(DebounceTimeoutProperty, value); }
        }

        public static readonly DependencyProperty DebounceTimeoutProperty =
            DependencyProperty.Register("DebounceTimeout", typeof(TimeSpan), typeof(GpioWorker), new PropertyMetadata(TimeSpan.Zero));

        GpioController gpioController;
        GpioPin gpioPin;

        public event EventHandler<ValueChangedEventArgs> ValueChanged;

        public void Start()
        {
            gpioController = GpioController.GetDefault();
            if (gpioController != null)
            {
                gpioPin = gpioController.OpenPin(Pin);
                var initialRead = gpioPin.Read();
                gpioPin.DebounceTimeout = DebounceTimeout;
                Debug.WriteLine($"Initial read for pin {Pin} is {initialRead}");
                Update(initialRead == GpioPinValue.High);
                gpioPin.ValueChanged += OnValueChanged;
            }
        }

        public void Stop()
        {
            Update(false);

            gpioController = null;
            if (gpioPin != null)
            { 
                gpioPin.ValueChanged -= OnValueChanged;
                gpioPin.Dispose();
                gpioPin = null;
            }
        }

        private void OnValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            Update(args.Edge == GpioPinEdge.RisingEdge);
        }

        public void Update(bool value)
        { 
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Debug.WriteLine($"Pin {Pin} Value {value}");
                Value = value;
                ValueChanged?.Invoke(this, value ? ValueTrue : ValueFalse);
            });
        }
    }
}
