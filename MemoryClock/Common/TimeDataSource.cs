using System;
using System.Collections.Generic;

namespace Common
{
    public class TimeDataSource : List<TimeSpan>
    {
        public TimeDataSource()
        {
            for (int i = 0; i < 24 * 4; ++i)
            {
                this.Add(TimeSpan.FromMinutes(i * 15));
            }
        }
    }
}
