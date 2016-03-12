using System;
using System.Threading;

namespace TimerQueueTimer
{
    // Represents an event in the event queue.
    class TimerQueueEvent : ICloneable
    {
        // The action to be invoked on time-out.
        public WaitCallback CallBack { get; set; }
        // The argument to be passed to action on time-out.
        public Object State { get; set; }
        // The time in future when the action gets invoked.
        public TimeSpan TimeOut { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
