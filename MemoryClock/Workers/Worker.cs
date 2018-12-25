using Common;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System.Threading;
using Windows.UI.Xaml;

namespace MemoryClock.Workers
{
    public interface IWorker
    {
        void Start();
        void Stop();

        bool IsEnabled { get; set; }
        bool IsAlwaysEnabled { get; }
        bool IsRaspberryPiOnly { get; }
    }

    public abstract class Worker : DependencyObject, IWorker
    {
        private IAsyncInfo AsyncInfo { get; set; }

        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(Worker), new PropertyMetadata(false));

        public Worker() { }

        public void Start()
        {
            if (IsEnabled && (AsyncInfo == null || AsyncInfo.Status != AsyncStatus.Started))
            {
                AsyncInfo = ThreadPool.RunAsync(DoWork);
            }
        }

        public void Stop()
        {
            AsyncInfo?.Cancel();
        }

        public bool IsAlwaysEnabled { get; protected set; } = false;
        public bool IsRaspberryPiOnly { get; protected set; } = false;

        public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(1);

        protected virtual void Setup() { }
        protected abstract bool DoWork();
        protected virtual void TearDown() { }

        private void DoWork(IAsyncAction operation)
        {
            Setup();

            while (operation.Status == AsyncStatus.Started)
            {
                try
                {
                    var shouldCancel = !DoWork();
                    if (shouldCancel)
                    {
                        operation.Cancel();
                    }
                }
                catch (Exception e)
                {
                    Logger.Log($"Worker {this.GetType()} raised an exception {e}");
                    operation.Cancel();
                }

                Task.Delay((int)Interval.TotalMilliseconds).Wait();
            }

            TearDown();
        }
    }
}
