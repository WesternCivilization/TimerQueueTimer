# TimerQueueTimer
Type for executing user-defined functions with a user-defined arguments at a user-defined time

TimerQueueTimer is one of the synchronization functions supported in Win32 API. see this [MSDN](https://msdn.microsoft.com/en-us/library/windows/desktop/ms682485(v=vs.85).aspx) page.

Role of TimerQueueTimer in nutshell is: Execute a user-defined function with a user-defined argument at a user-defined time.

TimerQueue in C++ allows user to register a routine with a special queue called timer queue. The routine will be executed by a thread after delay specified by user while registering the routine. We refer this routine as timer routine.

This is project is an atrempt to implement similar functionality in C#.

In nutshell we use priority queue to solve this problem. Each element of the queue contains callback address (the timer routine), reference to callback parameter and time in future when it should be fired. The 'time' is the priority.

The logic here is to have way to wake up the timer thread from another thread. When callback is added to the queue by main thread the timer thread will waken up and it look for the top element of priority queue, calculates the difference between current time and 'time' associated with the top element then sleeps until calculated timeout exceeds. When the timer thread is awaken by timeout it starts new thread from thread pool which invokes callback.

The [TimerQueue constructor](/TimerQueueTimer/TimerQueue.cs#L66) accepts one argument that defines the maximum number of timer routine you want to register with the timer queue.

[TimerQueue::Initialize](/TimerQueueTimer/TimerQueue.cs#L74) initializes the internal data structure associated with the timer queue, you need to call this function before start registering the timer routines.

You will use [TimerQueue::SetTimer](/TimerQueueTimer/TimerQueue.cs#L84) to register a timer routine with the timer queue. This method accepts reference to the function to be invoked, argument to be passed to this function on invocation and delay in milliseconds after which the function needs to be invoked.
