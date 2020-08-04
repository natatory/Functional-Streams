using System;

namespace Functional_Streams
{
    public class Stream<T>
    {
        public readonly T Head;
        public readonly Lazy<Stream<T>> Tail;
        private Stream()
        { }

        public Stream(T head, Lazy<Stream<T>> tail)
        {
            Head = head;
            Tail = tail;
        }


    }
}
