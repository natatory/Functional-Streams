using System;
using System.Collections.Generic;


namespace Functional_Streams
{
    public static class Stream
    {
        /*
            Your first task is to define a utility function which constructs a
            Stream given a head and a function returning a tail.
        */
        public static Stream<T> Cons<T>(T h, Func<Stream<T>> t)
        {

            return new Stream<T>(h, new Lazy<Stream<T>>(t));
        }

        // .------------------------------.
        // | Static constructor functions |
        // '------------------------------'

        // Construct a stream by repeating a value.
        public static Stream<T> Repeat<T>(T x)
        {
            return Cons(x, () => Repeat(x));
        }

        // Construct a stream by repeatedly applying a function.
        public static Stream<T> Iterate<T>(Func<T, T> f, T x)
        {
            return Cons(x, () => Iterate(f, f(x)));
        }

        // Construct a stream by repeating an enumeration forever.
        private static CycleStreamBuilder cycleSolution;
        public static Stream<T> Cycle<T>(IEnumerable<T> a)
        {
            cycleSolution = new CycleStreamBuilder();
            return cycleSolution.Cycle(a);
        }


        // Construct a stream by counting numbers starting from a given one.
        public static Stream<int> From(int x)
        {
            return Cons(x, () => From(x + 1));
        }

        // Same as From but count with a given step width.
        public static Stream<int> FromThen(int x, int d)
        {
            return Cons(x, () => FromThen(x + d, d));
        }

        // .------------------------------------------.
        // | Stream reduction and modification (pure) |
        // '------------------------------------------'

        /*
            Being applied to a stream (x1, x2, x3, ...), Foldr shall return
            f(x1, f(x2, f(x3, ...))). Foldr is a right-associative fold.
            Thus applications of f are nested to the right.
        */
        public static U Foldr<T, U>(this Stream<T> s, Func<T, Func<U>, U> f)
        {
            return f(s.Head, () => Foldr(s.Tail.Value, f));
        }

        // Filter stream with a predicate function.
        public static Stream<T> Filter<T>(this Stream<T> s, Predicate<T> p)
        {
            while (!p(s.Head))
            {
                s = s.Tail.Value;
            }
            return Cons(s.Head, () => Filter(s.Tail.Value, p));
        }

        // Returns a given amount of elements from the stream.
        private static TakeEnumBuilder takeBuilder;
        public static IEnumerable<T> Take<T>(this Stream<T> s, int n)
        {
            takeBuilder = new TakeEnumBuilder();
            return takeBuilder.Take(s, n);
        }

        // Drop a given amount of elements from the stream.
        public static Stream<T> Drop<T>(this Stream<T> s, int n)
        {
            if (n <= 0) return s;
            Stream<T> res = s;
            for (int i = 0; i < n; i++)
            {
                res = res.Tail.Value;
            }
            return res;
        }

        // Combine 2 streams with a function.
        public static Stream<R> ZipWith<T, U, R>(this Stream<T> s, Func<T, U, R> f, Stream<U> other)
        {
            return Cons(f(s.Head, other.Head), () => ZipWith(s.Tail.Value, f, other.Tail.Value));
        }

        // Map every value of the stream with a function, returning a new stream.
        public static Stream<U> FMap<T, U>(this Stream<T> s, Func<T, U> f)
        {
            return Cons(f(s.Head), () => FMap(s.Tail.Value, f));
        }

        // Return the stream of all fibonacci numbers.
        public static Stream<UInt64> Fib()
        {
            return Cons<UInt64>(0, () => Cons<UInt64>(1, () => Fib(0, 1)));
        }

        private static Stream<UInt64> Fib(UInt64 n, UInt64 n1)
        {
            return Cons<UInt64>(n + n1, () => Fib(n1, n + n1));
        }

        // Return the stream of all prime numbers.
        public static Stream<int> Primes()
        {
            return Cons(From(2).Head, () => Filter(From(2).Tail.Value, IsPrime));
        }

        private static bool IsPrime(int n)
        {
            if (n <= 2 || n % 2 == 0) return n == 2;
            for (int i = 3; i <= Math.Sqrt(n); i += 2)
                if (n % i == 0) return false;
            return true;
        }

    }
}
