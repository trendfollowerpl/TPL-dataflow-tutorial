using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ActionBlockExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var actionBlock = new ActionBlock<int>(n =>
            {
                Task.Delay(500).Wait();
                Console.WriteLine(n);
            });

            for (int i = 0; i < 10; i++)
            {
                actionBlock.Post(i);
                Console.WriteLine($"There are {actionBlock.InputCount} in the input queue");
            }

            Console.WriteLine("done");
            Console.ReadKey();
        }
    }
}
