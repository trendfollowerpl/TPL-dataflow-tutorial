using System;
using System.Linq;
using System.Threading.Tasks.Dataflow;

namespace BatchBlockExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var batchBlock = new BatchBlock<int>(3);
            for (int i = 0; i < 10; i++)
            {
                batchBlock.Post(i);
            }
            batchBlock.Complete();
            batchBlock.Post(10);

            for (int i = 0; i < 5; i++)
            {
                if (batchBlock.TryReceive(out var result))
                {
                    Console.Write($"Received batch {i}: ");
                    foreach (var r in result)
                    {
                        Console.Write(r + " ");
                    }
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("block finished");
                    break;
                }
            }

            Console.WriteLine("done");
            Console.ReadKey();
        }
    }
}
