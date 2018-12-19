#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace MemoryClock.Sensors
{
    public class TimeController : DependencyObject
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

        DispatcherTimer timer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
        
        public DateTime Now
        {
            get { return (DateTime)GetValue(NowProperty); }
            set { SetValue(NowProperty, value); }
        }

        public bool IsPaused
        {
            get { return timer.IsEnabled; }
            set
            {
                if (value)
                {
                    timer.Stop();
                }
                else
                {
                    timer.Start();
                }
            }
        }

        public event EventHandler<TickEventArgs> Tick;

        public static readonly DependencyProperty NowProperty =
            DependencyProperty.Register("Now", typeof(DateTime), typeof(TimeController), new PropertyMetadata(DateTime.Now));

        private TickEventArgs eventArgs = new TickEventArgs();

        public TimeController()
        {
            timer.Tick += OnTick;
        }

        public void Update(DateTime now)
        {
            Now = now;
            Tick?.Invoke(this, eventArgs.Set(Now));
        }

        private void OnTick(object sender, object e)
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => 
            {
                Update(DateTime.Now);
            });
        }
    }
}
