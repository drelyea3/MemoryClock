using Common;
using System;

namespace MemoryClock
{
    public class Settings
    {
        public int TickInterval { get; set; } = 100;

        // As the opacity values go from 0 to 1, the underlying screen gets darker.
        // 0 = brightest, 1 = darkest (can't see it at all)
        public double OverlayOpacityBright { get; set; } = 0.0;
        public double OverlayOpacityDim { get; set; } = 0.6;

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
                new Event<bool>() { Start = new TimeSpan(6,0,0), Value = false },
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
        }
    }
}