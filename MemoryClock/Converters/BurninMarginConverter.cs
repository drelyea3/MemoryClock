using System;
using System.Collections.Generic;
using Windows.UI.Xaml;

namespace MemoryClock.Converters
{
    class BurninMarginConverter : OneWayConverter<DateTime>
    {
        Queue<Thickness> margins = new Queue<Thickness>();
        Random rand = new Random();
        int lastInterval = -1;
        Thickness lastMargin;

        Queue<Thickness> CreateRandomMargins(int min, int max, int step)
        {
            var result = new Queue<Thickness>();

            var horizontal = new List<int>();
            var vertical = new List<int>();

            int totalMargin = max + min;

            for (var m = min; m <= max; m += step)
            {
                horizontal.Add(m);
                vertical.Add(m);
            }

            while (horizontal.Count > 0)
            {
                var randH = rand.Next(horizontal.Count);
                var randV = rand.Next(vertical.Count);

                var newH = horizontal[randH];
                var newV = vertical[randV];

                horizontal.RemoveAt(randH);
                vertical.RemoveAt(randV);

                var margin = new Thickness(newH, newV, totalMargin - newH, totalMargin - newV);
                result.Enqueue(margin);
            }
            return result;
        }

        protected override object Convert(DateTime value)
        {
            var interval = value.Minute / 5;
            if (interval != lastInterval)
            {
                lastInterval = interval;

                if (margins.Count == 0)
                {
                    margins = CreateRandomMargins(8, 96, 8);
                }

                var margin = margins.Dequeue();
                lastMargin = margin;
                //System.Diagnostics.Debug.WriteLine($"Burnin margin {margin}");
                return margin;
            }

            return lastMargin;
        }
    }
}
