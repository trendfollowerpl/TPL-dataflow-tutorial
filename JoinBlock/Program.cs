using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace JoinBlock
{
    class Program
    {
        static void Main(string[] args)
        {
            var broadcastBlock =
                new BroadcastBlock<int>(a => a,
                    new DataflowBlockOptions() { BoundedCapacity = 1 }
                );

            var a1 = new TransformBlock<int, int>(
                a =>
                {
                    Console.WriteLine($"Message {a} was processed by consumer 1");
                    Task.Delay(100).Wait();
                    return a;
                });

            var a2 = new TransformBlock<int, int>(
                a =>
                {
                    Console.WriteLine($"Message {a} was processed by consumer 2");
                    Task.Delay(100).Wait();
                    return a;
                });

            broadcastBlock.LinkTo(a1);
            broadcastBlock.LinkTo(a2);

            var joinblock = new JoinBlock<int, int>();
            a1.LinkTo(joinblock.Target1);
            a2.LinkTo(joinblock.Target2);

            var printBlock = new ActionBlock<Tuple<int,int>>(a => Console.WriteLine($"Message {a} was processed"));
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
