using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace BroadcastBlockExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var bufferBlock = new BufferBlock<int>(new DataflowBlockOptions() {BoundedCapacity = 1});
            var a1 = new ActionBlock<int>(
                a =>
                {
                    Console.WriteLine($"Message {a} was processed by consumer 1");
                    Task.Delay(100).Wait();
                },
                new ExecutionDataflowBlockOptions() {BoundedCapacity = 1});

            var a2 = new ActionBlock<int>(
                a =>
                {
                    Console.WriteLine($"Message {a} was processed by consumer 2");
                    Task.Delay(100).Wait();
                },
                new ExecutionDataflowBlockOptions() {BoundedCapacity = 1});

            bufferBlock.LinkTo(a1);
            bufferBlock.LinkTo(a2);

            for (int i = 0; i < 10; i++)
            {
                var i1 = i;
                bufferBlock.SendAsync(i)
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
