using System.Collections.Generic;
using System.Linq;


namespace Functional_Streams
{
    class CycleStreamBuilder
    {
        private object cycleEnumerable = null;
        public Stream<T> Cycle<T>(IEnumerable<T> a)
        {
            if (cycleEnumerable == null) cycleEnumerable = a;
            return Stream.Cons(a.First(), () => a.Count() > 1
              ? Cycle(a.Skip(1)) : Cycle((IEnumerable<T>)cycleEnumerable));
        }
    }
}
