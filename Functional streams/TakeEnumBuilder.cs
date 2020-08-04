using System.Collections.Generic;

namespace Functional_Streams
{
    class TakeEnumBuilder
    {
        private object res = null;
        public IEnumerable<T> Take<T>(Stream<T> s, int n)
        {
            if (n <= 0) return new List<T>() { };
            if (res == null) res = new List<T>();
            for (int i = 0; i < n; i++)
            {
                ((List<T>)res).Add(s.Head);
                s = s.Tail.Value;
            }
            return (List<T>)res;
        }
    }
}
