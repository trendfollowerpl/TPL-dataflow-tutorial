using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;


namespace ErrorHandlingExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var block = new TransformBlock<int, string>(n =>
             {
                 if (n == 5)
                 {
                     throw new ArgumentException("Sth went wrong");
                 }
                 Console.WriteLine($"Message {n} processed");
                 return n.ToString();
             });

            var printBlock = new ActionBlock<string>(s =>
                  Console.WriteLine($"block recived: {block.Receive()}")
            );

            block.LinkTo(printBlock, new DataflowLinkOptions(){PropagateCompletion = true});

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(block.Post(i) ? $"Message {i} accepted" : $"Message {i} rejected");
            }

            //for (int i = 0; i < 10; i++)
            //{
            //    Console.WriteLine($"block recived: {block.Receive()}");
            //}
            block.Complete();
            try
            {
                await printBlock.Completion;
            }
            catch (AggregateException e)
            {
                Console.WriteLine(e.Flatten().InnerExceptions);
            }
           

            Console.WriteLine("DONE");
            Console.ReadLine();
        }
    }
}
