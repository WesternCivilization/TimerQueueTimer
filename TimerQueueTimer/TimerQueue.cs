using System;
using System.Collections.Generic;
using System.Threading;

namespace TimerQueueTimer
{
    // Represents Timer-queue that enable you to specify
    // callback functions to be called at a specified time.
    public class TimerQueue
    {

        // Represents an event in the event (timer) queue.
        class TimerQueueEvent : ICloneable
        {
            // The routine to be invoked on time-out.
            public WaitCallback CallBack { get; set; }
            // The argument to be passed to routine on time-out.
            public Object State { get; set; }
            // The time in future when the routine gets invoked.
            public TimeSpan TimeOut { get; set; }

            public object Clone()
            {
                return this.MemberwiseClone();
            }
        }

        // Used to make the heap as min heap, we store the events
        // in the queue such that event with minium time-out must
        // be in the top.
        public class DoubleComparer : IComparer<double>
        {
            public int Compare(double value1, double value2)
            {
                return (int)(value1 - value2);
            }
        }

        // EventQueue uses a Min-Heap with Time-out as priority.
        Heap<double, TimerQueueEvent> eventQueue;

        // Wait handle for signalling, used by event processor thread
        // to wait for a new event to come or wait for an event to
        // time-out
        // Overhead involved in calling AutoResetEvent::Set is 1000
        // nano seconds (0.001 microseconds)
        AutoResetEvent autoResetEvent = new AutoResetEvent(false);

        // true for this flag indicates user called 'Initialize' hence
        // event thread is ready for processing.
        bool initialized = false;

        // The event processor thread.
        Thread eventThread;

        // Used to notify the event processor thread that new event request created and queued 
        TimerQueueEvent newEvent;

        // To synchronize access to event (timer) queue object between main thread
        // and event processor thread.
        readonly object synchNewEventLock = new object();

        // Creates new TimerQueue where queueSize is the maximum number
        // of events that can stay in the queue at any given point of
        // time.
        public TimerQueue(int queueSize)
        {
            eventQueue = new Heap<double, TimerQueueEvent>(queueSize,
                new DoubleComparer(), null);
            eventThread = new Thread(new ThreadStart(this.EventProcessor));
        }

        // Initializes the TimerQueue
        public void Initialize()
        {
            initialized = true;
            eventThread.Start();
        }

        // This timer expires at the specified wait time, when the timer
        // expires, the callback function is called. waitTimeInMs is
        // amount of time in milliseconds relative to the current time
        // that must elapse before the timer invokes callBack.
        public void SetTimer(WaitCallback callBack, Object state, double waitTimeInMs)
        {
            if (!initialized)
                throw new InvalidOperationException("method Initialize must be called before using the TimerQueue for scheduling actions");

            lock (synchNewEventLock)
            {
                // prepare a new event.
                newEvent = new TimerQueueEvent
                {
                    CallBack = callBack,
                    State = state,
                    TimeOut =
                    new TimeSpan(DateTime.Now.AddMilliseconds(waitTimeInMs).Ticks)
                };

                // add the new event to timer queue
                eventQueue.Push(newEvent.TimeOut.TotalMilliseconds, newEvent);

                // Signal the arrival of new event to event processor
                autoResetEvent.Set();
            };
        }

        // Returns the current time in milliseconds.
        private double NowInMilliSeconds
        {
            get
            {
                return TimeSpan.FromTicks(DateTime.Now.Ticks).TotalMilliseconds;
            }
        }

        private void EventProcessor()
        {
            // EventQueue is empty now, wait for an event to come.
            autoResetEvent.WaitOne();
            while (true)
            {
                TimerQueueEvent minEvent = null;

                lock (synchNewEventLock)
                {
                    minEvent = eventQueue.Peek();
                }

                if (minEvent == null)
                {
                    // EventQueue is empty, wait for an event to come.
                    autoResetEvent.WaitOne();
                }
                else
                {
                    // Calculate how much time we need to wait before processing
                    // event with minimum time-out and wait for that much time.
                    double waitTimeInMs =
                        minEvent.TimeOut.TotalMilliseconds - NowInMilliSeconds;
                    if (!autoResetEvent.WaitOne((int)waitTimeInMs))
                    {
                        // wait timed-out, invoke the callback associated with the
                        // event we waited for.
                        lock (synchNewEventLock)
                        {
                            minEvent = eventQueue.Pop();
                        }

                        ThreadPool.QueueUserWorkItem(minEvent.CallBack, minEvent.State);
                    }

                    // wait timed-out or wait released via signal (ManualResetEvent::Set)
                    // as a result of arrival of new event.
                }
            }
        }
    };
}
