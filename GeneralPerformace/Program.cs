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
            int counter = 0;
            int x;
            var ab = new ActionBlock<int>(i =>
            {
                counter++;
                x = i;
                if (i >= ITERS)
                {
                    are.Set();
                }
            }, new ExecutionDataflowBlockOptions { SingleProducerConstrained = false });

            for (int j = 0; j < 10; j++)
            {
                sw.Restart();
                Task[] tasks = new Task[6];

                for (int k = 0; k < 6; k++)
                {
                    tasks[k] = new TaskFactory().StartNew(() =>
                             {
                                 for (int i = 0; i <= ITERS / 6; i++)
                                 {
                                     ab.Post(i);
                                 }
                             });
                }

                Task.WaitAll(tasks);
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
