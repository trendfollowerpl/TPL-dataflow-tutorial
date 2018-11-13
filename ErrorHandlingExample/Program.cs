using System;
using System.Threading.Tasks.Dataflow;


namespace ErrorHandlingExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var block = new ActionBlock<int>(n =>
            {
                if (n==5)
                {
                    throw new ArgumentException("Sth went wrong");
                }
                Console.WriteLine($"Message {n} posted");
            });

            for (int i = 0; i < 10; i++)
            {
                block.Post(i);
            }

            Console.WriteLine("DONE");
            Console.ReadLine();
        }
    }
}
