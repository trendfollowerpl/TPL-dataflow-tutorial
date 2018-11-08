using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TransformBlockExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var transformBlock = new TransformBlock<int, string>(i =>
           {
               Task.Delay(500).Wait();
               return i.ToString();
           }, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 2 });

            for (int i = 0; i < 10; i++)
            {
                transformBlock.Post(i);
                Console.WriteLine($"Number of messages in the input queue: {transformBlock.InputCount}");
            }

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"Number of messages in the input queue: {transformBlock.OutputCount}");
                var result = transformBlock.Receive();
                Console.WriteLine($"Recived: {result}");
            }

            Console.WriteLine("done");
            Console.ReadKey();
        }
    }
}
