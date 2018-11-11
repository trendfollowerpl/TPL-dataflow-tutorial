using System;
using System.Threading.Tasks.Dataflow;

namespace LinkToWithPropagation
{
    public static class LinkToWithPropagationExtention
    {
        public static IDisposable LinkToWithPropagation<T>(this ISourceBlock<T> source, ITargetBlock<T> target)
        {
            return source.LinkTo(target, new DataflowLinkOptions { PropagateCompletion = true });
        }
    }
}
