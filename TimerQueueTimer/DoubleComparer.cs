using System.Collections.Generic;

namespace TimerQueueTimer
{
    // Comparer used to make the heap as min heap, we store the events
    // in the queue such that event with minimum time-out must
    // be in the top.
    public class DoubleComparer : IComparer<double>
    {
        public int Compare(double value1, double value2)
        {
            return (int)(value1 - value2);
        }

    }
}
