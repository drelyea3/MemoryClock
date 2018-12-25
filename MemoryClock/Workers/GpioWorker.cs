#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

using Common;
using System;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;

namespace MemoryClock.Workers
{
    public class GpioWorker : DependencyObject, IWorker
    {
        GpioController gpioController;
        GpioPin gpioPin;

        public bool Value
        {
            get { return (bool)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(bool), typeof(GpioWorker), new PropertyMetadata(false));

        public bool NotValue
        {
            get { return (bool)GetValue(NotValueProperty); }
            set { SetValue(NotValueProperty, value); }
        }

        public static readonly DependencyProperty NotValueProperty =
            DependencyProperty.Register("NotValue", typeof(bool), typeof(GpioWorker), new PropertyMetadata(true));

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

        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(GpioWorker), new PropertyMetadata(false));


        public void Start()
        {
            if (IsEnabled)
            {
                gpioController = GpioController.GetDefault();
                if (gpioController != null)
                {
                    gpioPin = gpioController.OpenPin(Pin);
                    var initialRead = gpioPin.Read();
                    gpioPin.DebounceTimeout = DebounceTimeout;
                    Logger.Log($"Initial read for pin {Pin} is {initialRead}");
                    Update(initialRead == GpioPinValue.High);
                    gpioPin.ValueChanged += OnValueChanged;
                }
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

        public bool IsAlwaysEnabled { get; } = false;
        public bool IsRaspberryPiOnly { get; } = true;

        private void OnValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            Update(args.Edge == GpioPinEdge.RisingEdge);
        }

        public void Update(bool value)
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Logger.Log($"Pin {Pin} Value {value}");
                Value = value;
                NotValue = !value;
            });
        }
    }
}
