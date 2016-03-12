using System;
using System.Threading;

namespace TimerQueueTimer
{
    class Program
    {
        static void Main(string[] args)
        {
            TimerQueue timerQueue = new TimerQueue(10);
            timerQueue.Initialize();

            timerQueue.SetTimer(CallMeBack, " From callback after 5 sec", 5000);
            timerQueue.SetTimer(CallMeBack, " From callback after 2 sec", 2000);
            timerQueue.SetTimer(CallMeBack, " From callback after 8 sec", 8000);

            Console.WriteLine("Main application is going to wait for 15 seconds");
            Thread.Sleep(15000);
            Console.WriteLine("Main application finished waiting, adding few more timer events");

            timerQueue.SetTimer(CallMeBack, " From callback after 13 sec", 13000);
            timerQueue.SetTimer(CallMeBack, " From callback after 9 sec", 9000);
            timerQueue.SetTimer(CallMeBack, " From callback after 4 sec", 4000);

            Console.ReadLine();
        }

        static void CallMeBack(Object state)
        {
            Console.WriteLine((string)state);
        }
    }

}
