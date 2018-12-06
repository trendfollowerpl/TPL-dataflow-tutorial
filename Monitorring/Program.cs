using System;
using System.Threading.Tasks.Dataflow;
using System.Threading.Tasks;
using LinkToWithPropagation;

namespace Monitorring
{
    class Program
    {
        static void Main(string[] args)
        {
            var random = new Random();
            var broadcastBlock = new BroadcastBlock<int>(null);
            var transformPositive = new TransformBlock<int, int>(x =>
            {
                Task.Delay(random.Next(200)).Wait();
                return x;
            });
            var transformNegative = new TransformBlock<int, int>(x =>
            {
                Task.Delay(random.Next(300)).Wait();
                return x * -1;
            });
            var joinBlock = new JoinBlock<int, int>();
            var sumBlock = new ActionBlock<(int, int)>(tuple =>
            {
                Console.WriteLine($"{tuple.Item1}+{tuple.Item2}={tuple.Item1+tuple.Item2}");
            });

            broadcastBlock.LinkToWithPropagation(transformPositive);
            broadcastBlock.LinkToWithPropagation(transformNegative);
            transformNegative.LinkToWithPropagation(joinBlock.Target1);
            transformPositive.LinkToWithPropagation(joinBlock.Target2);

        }
    }
}
