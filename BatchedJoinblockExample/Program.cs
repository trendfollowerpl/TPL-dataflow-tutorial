using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Linq;

namespace BatchedJoinblockExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var broadcastBlock = new BroadcastBlock<int>(a => a, new DataflowBlockOptions() { BoundedCapacity = 1 });

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

            var joinblock = new BatchedJoinBlock<int, int>(3);
            a1.LinkTo(joinblock.Target1);
            a2.LinkTo(joinblock.Target2);

            var printBlock = new ActionBlock<Tuple<IList<int>, IList<int>>>(
                a => Console.WriteLine($"Message [{string.Join(",", a.Item1)}]" +
                $",[{string.Join(",", a.Item2)}]"));
            joinblock.LinkTo(printBlock);

            for (int i = 0; i < 10; i++)
            {
                var i1 = i;
                broadcastBlock.SendAsync(i)
                    .ContinueWith(a =>
                    {
                        Console.WriteLine(a.Result ? $"Messgae {i1} was accepted" : $"Messgae {i1} was rejected");
                    });
            }

            Console.WriteLine("done");
            Console.ReadLine();
        }

    }
}
