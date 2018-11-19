using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using LinkToWithPropagation;

namespace Customblock.Inheritance
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var broadcastBlock =
                new GuarantededDeliveryBroadcastBlock<int>(a => a);

            var a1 = new ActionBlock<int>(
                a =>
                {
                    Console.WriteLine($"Message {a} was processed by consumer 1");
                    Task.Delay(500).Wait();
                }, new ExecutionDataflowBlockOptions() { BoundedCapacity = 1 });

            var a2 = new ActionBlock<int>(
                a =>
                {
                    Console.WriteLine($"Message {a} was processed by consumer 2");
                    Task.Delay(150).Wait();
                }, new ExecutionDataflowBlockOptions() { BoundedCapacity = 1 });

            broadcastBlock.LinkToWithPropagation(a1);
            broadcastBlock.LinkToWithPropagation(a2);

            for (int i = 0; i < 10; i++)
            {
                await broadcastBlock.SendAsync(i);
            }

            broadcastBlock.Complete();
            await a1.Completion;
            await a2.Completion;

            Console.WriteLine("done");
            Console.ReadLine();
        }
    }

    class GuarantededDeliveryBroadcastBlock<T> : IPropagatorBlock<T, T>
    {
        private BroadcastBlock<T> _broadcastBlock;
        private Task _completion;

        public GuarantededDeliveryBroadcastBlock(Func<T, T> cloningFunc)
        {
            _broadcastBlock = new BroadcastBlock<T>(cloningFunc);
            _completion = _broadcastBlock.Completion;
        }

        public DataflowMessageStatus OfferMessage(DataflowMessageHeader messageHeader, T messageValue, ISourceBlock<T> source,
            bool consumeToAccept)
        {
            return ((ITargetBlock<T>)_broadcastBlock).OfferMessage(messageHeader, messageValue, source, consumeToAccept);
        }

        public void Complete()
        {
            _broadcastBlock.Complete();
        }

        public void Fault(Exception exception)
        {
            ((ITargetBlock<T>)_broadcastBlock).Fault(exception);
        }

        public Task Completion => _completion;
        public T ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<T> target, out bool messageConsumed)
        {
            throw new NotSupportedException("producer is a buffer block");
        }

        public IDisposable LinkTo(ITargetBlock<T> target, DataflowLinkOptions linkOptions)
        {
            var bufferBlock = new BufferBlock<T>();
            var l1 = _broadcastBlock.LinkTo(bufferBlock, linkOptions);
            var l2 = bufferBlock.LinkTo(target, linkOptions);

            _completion.ContinueWith(_ => bufferBlock.Completion);
            return new DisposableDisposer(l1, l2);
        }

        public void ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<T> target)
        {
            throw new NotSupportedException("producer is a buffer block");
        }

        public bool ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<T> target)
        {
            throw new NotSupportedException("producer is a buffer block");
        }
    }

    class DisposableDisposer : IDisposable
    {
        private readonly IDisposable[] _disposables;

        public DisposableDisposer(params IDisposable[] disposables)
        {
            _disposables = disposables;
        }
        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}
