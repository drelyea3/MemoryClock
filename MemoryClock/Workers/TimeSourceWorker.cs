#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }

        public DateTime Now
        {
            get { return (DateTime)GetValue(NowProperty); }
            set { SetValue(NowProperty, value); }
        }
        public static readonly DependencyProperty NowProperty =
            DependencyProperty.Register("Now", typeof(DateTime), typeof(TimeSourceWorker), new PropertyMetadata(DateTime.Now));

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
            Now = now;
            Tick?.Invoke(this, eventArgs.Set(Now));
        }
    }
}
