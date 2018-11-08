using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace TransformManyBlocks
{
    class Program
    {
        static void Main(string[] args)
        {
            var transformManyBlock = new TransformManyBlock<int, string>(
                i => FindEvenNumbers(i),
                new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 5 }
                );
            var printBlock = new ActionBlock<string>(a => Console.WriteLine($"Received message: {a}"));
            DataflowBlock.LinkTo(transformManyBlock, printBlock);

            for (int i = 0; i < 10; i++)
            {
                transformManyBlock.Post(i);
            }

            Console.WriteLine("done");
            Console.ReadKey();
        }

        private static IEnumerable<string> FindEvenNumbers(int number)
        {
            for (int i = 0; i < number; i++)
            {
                if (i % 2 == 0)
                {
                    yield return $"{number}: {i}";
                }
            }
        }
    }
}
