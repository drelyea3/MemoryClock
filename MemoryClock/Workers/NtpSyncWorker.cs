using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryClock.Workers
{
    public class NtpSyncWorker : Worker
    {
        NtpClient client = new NtpClient();
        protected override bool DoWork()
        {
            var now = DateTime.Now;
            var task = client.GetNetworkTimeAsync();
            task.Wait();
            if (task.IsCompletedSuccessfully)
            { 
                var result = task.Result.ToLocalTime();
                var drift = result - now; // If device clock is slow, will be positive
                Logger.Log($"NtpSyncWorker returned {result} DateTime.Now {now} drift {drift}");
            }
            return true;
        }
    }
}
