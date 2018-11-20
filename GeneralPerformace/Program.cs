using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace GeneralPerformace
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            const int ITERS = 6 * 1000 * 1000;
            var are = new AutoResetEvent(false);

            var ab = new ActionBlock<int>(i =>
            {
                if (i == ITERS)
                {
                    are.Set();
                }
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = ExecutionDataflowBlockOptions.Unbounded });

            for (int j = 0; j < 10; j++)
            {
                sw.Restart();
                for (int i = 0; i <= ITERS; i++)
                {
                    ab.Post(i);
                }
                are.WaitOne();
                sw.Stop();
                Console.WriteLine("Messages / sec: {0:N0}", ITERS / sw.Elapsed.TotalSeconds);
            }


            Console.WriteLine("Task");
            for (int j = 0; j < 10; j++)
            {
                sw.Restart();

                new TaskFactory().StartNew(() =>
                {
                    for (int i = 0; i <= ITERS; i++)
                    {
                        if (i == ITERS)
                        {
                            are.Set();
                        }
                    }
                });
                are.WaitOne();
                sw.Stop();
                Console.WriteLine("Messages / sec: {0:N0}", ITERS / sw.Elapsed.TotalSeconds);
            }


            Console.WriteLine("done");
            Console.ReadLine();
        }
    }
}
