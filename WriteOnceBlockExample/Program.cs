using System;
using System.Threading.Tasks.Dataflow;

namespace WriteOnceBlockExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var block = new WriteOnceBlock<int>(a => a);
            var print = new ActionBlock<int>(a => Console.WriteLine($"Messgae {a} was recived"));

            for (int i = 0; i < 10; i++)
            {
                if (block.Post(i))
                {
                    Console.WriteLine($"Messgae was {i} accepted");
                }
                else
                {
                    Console.WriteLine($"Messgae was {i} REJECTED");
                }
            }

            block.LinkTo(print);
          
            Console.WriteLine("done");
            Console.ReadKey();
        }
    }
}
