#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

using Common;
using System;
using Windows.UI.Xaml;

namespace MemoryClock.Workers
{
    public class NtpSyncWorker : Worker
    {
        NtpClient client = new NtpClient();

        public TimeSpan Drift
        {
            get { return (TimeSpan)GetValue(DriftProperty); }
            set { SetValue(DriftProperty, value); }
        }

        public static readonly DependencyProperty DriftProperty =
            DependencyProperty.Register("Drift", typeof(TimeSpan), typeof(NtpSyncWorker), new PropertyMetadata(TimeSpan.Zero));

        protected override bool DoWork()
        {
            var now = DateTime.Now;
            var task = client.GetNetworkTimeAsync();
            task.Wait();
            if (task.IsCompletedSuccessfully)
            {
                var result = task.Result.ToLocalTime();
                var drift = result - now; // If device clock is slow, will be positive
                Update(drift);
                Logger.Log($"NtpSyncWorker returned {result} DateTime.Now {now} drift {drift}");
            }
            return true;
        }

        private void Update(TimeSpan drift)
        {
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                var newDrift = new TimeSpan(drift.Days, drift.Hours, drift.Minutes, drift.Seconds);
                Drift = newDrift;
            });
        }
    }
}
