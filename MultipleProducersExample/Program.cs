using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using LinkToWithPropagation;

namespace MultipleProducersExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var producer1 = new TransformBlock<string, string>(async n =>
               {
                   await Task.Delay(150);
                   return n;
               });

            var producer2 = new TransformBlock<string, string>(n =>
            {
                Task.Delay(500).Wait();
                return n;
            });

            var printBlock = new ActionBlock<string>(n => Console.WriteLine(n));

            producer1.LinkTo(printBlock);
            producer2.LinkTo(printBlock);

            for (int i = 0; i < 10; i++)
            {
                producer1.Post($"Producer 1 message: {i}");
                producer2.Post($"Producer 2 message: {i}");
            }

            producer1.Complete();
            producer2.Complete();
            await Task.WhenAll(new[] { producer1.Completion, producer2.Completion })
                .ContinueWith(_ => printBlock.Complete())
                .ContinueWith(_ => printBlock.Completion);
                
            Console.WriteLine("done");
            Console.ReadLine();
        }
    }
}
