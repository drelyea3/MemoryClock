using Common;
using System;
using System.Diagnostics;
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
    }

    public abstract class Worker : DependencyObject, IWorker
    {
        private IAsyncInfo AsyncInfo { get; set; }

        public void Start()
        {
            if (AsyncInfo == null || AsyncInfo.Status != AsyncStatus.Started)
            {
                AsyncInfo = ThreadPool.RunAsync(DoWork);
            }
        }

        public void Stop()
        {
            AsyncInfo?.Cancel();
        }

        public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(1);

        protected abstract bool DoWork();

        private void DoWork(IAsyncAction operation)
        {
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
        }
    }
}
