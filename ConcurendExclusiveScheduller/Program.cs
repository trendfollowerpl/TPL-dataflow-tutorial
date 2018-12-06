using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using LinkToWithPropagation;

namespace ConcurendExclusiveScheduller
{
    class Program
    {
        static int shared = 0;
        static async Task Main(string[] args)
        {
            var scheduler = new ConcurrentExclusiveSchedulerPair();
            var inputBlock = new BroadcastBlock<int>(a => a);

            Action<int> actionBlockFunction = a =>
            {
                var random = new Random();
                int counterValue = GetSharedObjectValue();
                Task.Delay(random.Next(300)).Wait();
                Console.WriteLine($"counter value was{counterValue}, Now it is :{shared}, it will be set to : {counterValue + 1}");
                SetSharedObjectValue(counterValue + 1);
            };

            var incrementBlock1 = new ActionBlock<int>(actionBlockFunction, new ExecutionDataflowBlockOptions { TaskScheduler = scheduler.ExclusiveScheduler });
            var incrementBlock2 = new ActionBlock<int>(actionBlockFunction, new ExecutionDataflowBlockOptions { TaskScheduler = scheduler.ExclusiveScheduler });

            inputBlock.LinkToWithPropagation(incrementBlock1);
            inputBlock.LinkToWithPropagation(incrementBlock2);

            for (int i = 0; i < 10; i++)
            {
                inputBlock.Post(i);
            }
            inputBlock.Complete();
            await incrementBlock1.Completion;
            await incrementBlock2.Completion;

            Console.WriteLine($"shared object vlaue: {GetSharedObjectValue()}");
            Console.ReadLine();
        }

        private static void SetSharedObjectValue(int v)
        {
            shared = v;
        }

        private static int GetSharedObjectValue()
        {
            return shared;
        }
    }
}
