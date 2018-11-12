using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace LinkToWithMaxMessagesExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var bufferBlock = new BufferBlock<int>(new DataflowBlockOptions() { BoundedCapacity = 1 });
            var a1 = new ActionBlock<int>(
                a =>
                {
                    Console.WriteLine($"Message {a} was processed by consumer 1");
                    Task.Delay(100).Wait();
                });

            var a2 = new ActionBlock<int>(
                a =>
                {
                    Console.WriteLine($"Message {a} was processed by consumer 2");
                    Task.Delay(100).Wait();
                });

            bufferBlock.LinkTo(a1);
            bufferBlock.LinkTo(a2, new DataflowLinkOptions { Append = false, MaxMessages = 5 });

            for (int i = 0; i < 10; i++)
            {
                await bufferBlock.SendAsync(i);
            }

            Console.WriteLine("done");
            Console.ReadLine();
        }
    }
}
