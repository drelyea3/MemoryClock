using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
