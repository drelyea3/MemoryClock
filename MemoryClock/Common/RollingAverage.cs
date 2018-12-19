using System.Linq;

namespace MemoryClock
{
    public class RollingAverage
    {
        double[] buffer;
        int bufferIndex;

        public RollingAverage(int size, double fill = default(double))
        {
            buffer = new double[size];
            if (fill != default(double))
            {
                for (int index = 0; index < size; ++index)
                {
                    buffer[index] = fill;
                }
            }
        }

        public void Update(double value)
        {
            buffer[bufferIndex] = value;
            bufferIndex = (bufferIndex + 1) % buffer.Length;
        }

        public double GetAverage()
        {
            return buffer.Average();
        }
    }
}
