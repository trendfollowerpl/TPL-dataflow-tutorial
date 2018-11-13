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
            var block = new ActionBlock<int>(n =>
            {
                if (n == 5)
                {
                    throw new ArgumentException("Sth went wrong");
                }
                Console.WriteLine($"Message {n} processed");
            });

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(block.Post(i) ? $"Message {i} accepted" : $"Message {i} rejected");
            }

            
            Console.WriteLine($"Input queue size: {block.InputCount}");
            await block.Completion;

            Console.WriteLine("DONE");
            Console.ReadLine();
        }
    }
}
