#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

using System;
using Windows.UI.Xaml;

namespace MemoryClock.Workers
{
    public class TimeSourceWorker : Worker
    {
        public class TickEventArgs : EventArgs
        {
            public DateTime Now { get; private set; }
            public TickEventArgs Set(DateTime now)
            {
                Now = now;
                return this;
            }
        }

        private TickEventArgs eventArgs = new TickEventArgs();

        public event EventHandler<TickEventArgs> Tick;

        public TimeSourceWorker()
        {
            Interval = Global.Settings.TickInterval;
            IsAlwaysEnabled = true;
        }

        public DateTime Now
        {
            get { return (DateTime)GetValue(NowProperty); }
            set { SetValue(NowProperty, value); }
        }
        public static readonly DependencyProperty NowProperty =
            DependencyProperty.Register("Now", typeof(DateTime), typeof(TimeSourceWorker), new PropertyMetadata(DateTime.Now));

        public TimeSpan Adjustment
        {
            get { return (TimeSpan)GetValue(AdjustmentProperty); }
            set { SetValue(AdjustmentProperty, value); }
        }

        public static readonly DependencyProperty AdjustmentProperty =
            DependencyProperty.Register("Adjustment", typeof(TimeSpan), typeof(TimeSourceWorker), new PropertyMetadata(TimeSpan.Zero));

        protected override bool DoWork()
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Update(DateTime.Now);
            });
            return true;
        }

        public void Update(DateTime now)
        {
            Now = now + Adjustment;
            Tick?.Invoke(this, eventArgs.Set(Now));
        }
    }
}
