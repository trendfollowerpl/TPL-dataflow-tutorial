using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CompletionExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var broadcastBlock =
                new BroadcastBlock<int>(a => a,
                    new DataflowBlockOptions() { BoundedCapacity = 1 }
                );

            var a1 = new TransformBlock<int, int>(
                a =>
                {
                    Console.WriteLine($"Message {a} was processed by consumer 1");
                    if (a % 2 == 0)
                    {
                        Task.Delay(300).Wait();
                    }
                    else
                    {
                        Task.Delay(50).Wait();
                    }
                    return a * -1;
                }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 3 });

            var a2 = new TransformBlock<int, int>(
                a =>
                {
                    Console.WriteLine($"Message {a} was processed by consumer 2");
                    if (a % 2 != 0)
                    {
                        Task.Delay(300).Wait();
                    }
                    else
                    {
                        Task.Delay(50).Wait();
                    }
                    return a;
                }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 3 });

            broadcastBlock.LinkTo(a1);
            broadcastBlock.LinkTo(a2);

            var joinblock = new JoinBlock<int, int>();
            a1.LinkTo(joinblock.Target1);
            a2.LinkTo(joinblock.Target2);

            var printBlock = new ActionBlock<Tuple<int, int>>(
                a => Console.WriteLine($"Message {a} was processed. Sum: {a.Item1 + a.Item2}")
                );
            joinblock.LinkTo(printBlock);

            for (int i = 0; i < 10; i++)
            {
                await broadcastBlock.SendAsync(i);
            }

            Console.WriteLine("done");
        }
    }
}

