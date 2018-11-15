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
            var inputBlock = new TransformBlock<int, int>(n =>
            {
                if (n == 2)
                {
                    throw new Exception("exception thrown from inputBlock");
                }
                return n;
            });
            var increasingBlock = CreateFilteringBlock<int>();
            var printBlock = new ActionBlock<int>(n => Console.WriteLine($"Message {n} recived"));

            inputBlock.LinkToWithPropagation(increasingBlock);
            increasingBlock.LinkToWithPropagation(printBlock);

            inputBlock.Post(1);
            inputBlock.Post(2);
            inputBlock.Post(1);
            inputBlock.Post(3);
            inputBlock.Post(4);
            inputBlock.Post(2);

            inputBlock.Complete();
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
            target.Completion.ContinueWith(a =>
            {
                if (a.IsFaulted)
                {
                    ((ITargetBlock<T>)source).Fault(a.Exception);
                }
                else
                {
                    source.Complete();
                }
            });

            return DataflowBlock.Encapsulate(target, source);
        }

    }
}
