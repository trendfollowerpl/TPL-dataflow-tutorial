using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using LinkToWithPropagation;

namespace CustomBlockEncapsulate
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var increasingBlock = CreateFilteringBlock<int>();

            var printBlock = new ActionBlock<int>(n => Console.WriteLine($"Message {n} recived"));

            increasingBlock.LinkToWithPropagation(printBlock);

            increasingBlock.Post(1);
            increasingBlock.Post(2);
            increasingBlock.Post(1);
            increasingBlock.Post(3);
            increasingBlock.Post(4);
            increasingBlock.Post(2);

            await printBlock.Completion;

            Console.WriteLine("done");
            Console.ReadLine();
        }

        public static IPropagatorBlock<T, T> CreateFilteringBlock<T>()
            where T : IComparable<T>, new()
        {
            T maxElement = default;
            var source = new BufferBlock<T>();
            var target = new ActionBlock<T>(async item =>
            {
                if (item.CompareTo(maxElement) > 0)
                {
                    await source.SendAsync(item);
                    maxElement = item;
                }
            });

            return DataflowBlock.Encapsulate(target, source);
        }

    }
}
