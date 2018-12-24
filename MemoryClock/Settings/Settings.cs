using Common;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;

namespace MemoryClock.Settings
{
    public class Settings
    {
        public List<string> EnabledWorkers = new List<string>();

        public TimeSpan TickInterval { get; set; } = TimeSpan.FromMilliseconds(500);

        public double MaxLux { get; set; } = 50.0;
        public double DayBrightness { get; set; } = 1.0;
        public double NightBrightness { get; set; } = 0.2;
        public TimeSpan LightSensorInterval { get; set; } = TimeSpan.FromSeconds(2);

        public QuarterHourlySchedule<bool> IsDim { get; set; } = new QuarterHourlySchedule<bool>();
        public QuarterHourlySchedule<bool> IsNoPhone { get; set; } = new QuarterHourlySchedule<bool>();
        public QuarterHourlySchedule<string> Message { get; set; } = new QuarterHourlySchedule<string>();

        public Settings()
        {
        }

        public void LoadDefaults()
        {
            IsDim.Events = new Event<bool>[]
            {
                new Event<bool>() { Start = new TimeSpan(7,0,0), Value = false },
                new Event<bool>() { Start = new TimeSpan(20,0,0), Value = true },
            };

            IsNoPhone.Events = new Event<bool>[]
            {
                new Event<bool>() { Start = new TimeSpan(8,0,0), Value = false },
                new Event<bool>() { Start = new TimeSpan(22,0,0), Value = true },
            };

            Message.Events = new Event<string>[]
            {
                new Event<string>(new TimeSpan(6,0,0), "Morning"),
                new Event<string>(new TimeSpan(12,0,0), "Afternoon"),
                new Event<string>(new TimeSpan(18,0,0), "Evening"),
                new Event<string>(new TimeSpan(20,0,0), "Night - time to sleep"),
            };

            EnabledWorkers.Clear();
            foreach (var worker in ((App)App.Current).GetWorkers())
            {
                if (worker.Item2.IsAlwaysEnabled || !worker.Item2.IsRaspberryPiOnly)
                {
                    EnabledWorkers.Add(worker.Item1);
                }
            }
        }
    }
}