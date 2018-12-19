using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public class Schedule<T>
    {

        private T[] values;
        List<Event<T>> __events__ = new List<Event<T>>();

        public Schedule(int size)
        {
            Size = size;
        }

        [JsonIgnore]
        public int Size { get; private set; }

        public IReadOnlyList<Event<T>> Events
        {
            get { return __events__; }
            set
            {
                __events__ = new List<Event<T>>(value);
                values = null;
            }
        }

        public void Add(Event<T> item)
        {
            __events__.Add(item);
            values = null;
        }

        public void Clear()
        {
            __events__.Clear();
            values = null;
        }

        public T GetValue(int interval)
        {
            if (values == null)
            {
                values = ConfigureEvents();
            }

            return values[interval];
        }

        public T GetValue(TimeSpan tod)
        {
            int segmentSize = (24 * 60) / Size;
            int segment = (int) tod.TotalMinutes / segmentSize;
            return GetValue(segment);
        }

        private T[] ConfigureEvents()
        {
            var result = new T[Size];

            var sorted = (from Event<T> e in __events__ orderby e.Start select e).ToList();

            if (sorted.Count == 1)
            {
                for (int index = 0; index < result.Length; ++index)
                {
                    result[index] = sorted[0].Value;
                }
            }
            else if (sorted.Count > 1)
            {
                var lastEventIndex = sorted.Count - 1;
                var eventIndex = 0;

                for (int index = 0; index < result.Length; ++index)
                {
                    if (eventIndex >= sorted.Count || index < sorted[eventIndex].GetInterval(Size))
                    {
                        // Fill after the last event or before the first event
                        result[index] = sorted[lastEventIndex].Value;
                    }
                    else if (index == sorted[eventIndex].GetInterval(Size))
                    {
                        result[index] = sorted[eventIndex].Value;
                        lastEventIndex = eventIndex;
                        ++eventIndex;
                    }
                }
            }

            return result;
        }
    }

    public class QuarterHourlySchedule<T> : Schedule<T>
    {
        public QuarterHourlySchedule()
            : base(24 * 4)
        {
        }
    }

    public class Event<T>
    {
        public Event() { }

        public Event(TimeSpan start, T value)
        {
            Start = start;
            Value = value;
        }

        public TimeSpan Start { get; set; }
        public T Value { get; set; }

        public int GetInterval(int size)
        {
            int intervalSize = (24 * 60) / size;
            int interval = (int)Start.TotalMinutes / intervalSize;
            return interval;
        }
    }
}
