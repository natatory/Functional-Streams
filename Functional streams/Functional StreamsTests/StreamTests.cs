using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;



namespace Functional_Streams
{
    [TestClass]
    public class StreamTests
    {
        [TestMethod]
        public void RepeatTest_SequenceOfSameInteger()
        {
            //arrange
            var v = new Random().Next();
            //act
            var s = Stream.Repeat(v);
            //assert
            for (var i = 0; i < 100; i++)
            {
                Assert.AreEqual(v, s.Head);
                s = s.Tail.Value;
            }
        }

        [TestMethod]
        public void IterateTest_SequenceWithExponentiationOfIntegers()
        {
            //arrange
            long multiplier = new Random().Next(9);
            Func<long, long> multiply = x => x * multiplier;
            long multiplied = multiplier;
            //act
            var expStream = Stream.Iterate(multiply, multiplied);
            //assert
            for (var i = 0; i < 10; i++)
            {
                Assert.AreEqual(multiplied, expStream.Head);
                expStream = expStream.Tail.Value;
                multiplied = multiply(multiplied);
            }
        }

        [TestMethod]
        public void IterateTest_SequenceSameOfIntegerWithWhitespaces()
        {
            //arrange
            Func<string, string> concatenate = x => x + " ";
            var concatenated = "";
            //act
            var addWSStream = Stream.Iterate(concatenate, concatenated);
            //assert
            for (var i = 0; i < 10; i++)
            {
                Assert.AreEqual(concatenated, addWSStream.Head);
                addWSStream = addWSStream.Tail.Value;
                concatenated = concatenate(concatenated);
            }
        }

        [TestMethod]
        public void CycleTest_RepeatEnumerable()
        {
            //arrange
            var r = new Random();
            var a = Enumerable.Range(0, 20).Select(i => r.Next()).ToArray();
            //act
            var s = Stream.Cycle(a);
            //assert
            for (var i = 0; i < 100; i++)
            {
                Assert.AreEqual(a[i % a.Length], s.Head, "cycle should repeat the enumerable");
                s = s.Tail.Value;
            }
        }

        [TestMethod]
        public void FromTest_CountingNumbersStartingFromGivenOne()
        {
            //arrange
            var v = new Random().Next();
            //act
            var s = Stream.From(v);
            //assert
            for (var i = v; i < v + 100; i++)
            {
                Assert.AreEqual(i, s.Head);
                s = s.Tail.Value;
            }
        }

        [TestMethod]
        public void FromThenTest_CountingNumbersStartingFromGivenOneWithStep()
        {
            //arrange
            var r = new Random();
            var v = r.Next();
            var d = r.Next(200);
            //act
            var s = Stream.FromThen(v, d);
            //assert
            for (var i = v; i < v + 100 * d; i += d)
            {
                Assert.AreEqual(i, s.Head);
                s = s.Tail.Value;
            }
        }

        [TestMethod]
        public void FoldrTest_RightAssociativeFold()
        {
            //arrange
            var v = new Random().Next();
            //act
            var a = Stream.Repeat(v).Foldr<int, Stream<int>>((x, r) => Stream.Cons(x + 1, () => r())).Take(10).ToArray();
            //assert
            for (var i = 0; i < 10; i++)
            {
                Assert.AreEqual(a[i], v + 1, "folding should work, with lazy tail");
            }
        }

        [TestMethod]
        public void FilterTest_SequenceWithPredicateFuncOdd()
        {
            //arrange
            var v = new Random().Next();
            //act
            var a = Stream.From(v).Filter(x => x % 2 == 0).Take(10).ToArray();
            //assert
            foreach (var i in a)
            {
                Assert.AreEqual(0, i % 2);
            }
        }

        [TestMethod]
        public void TakeTest_ReturnAmountOfElementsFromStream()
        {
            //arange
            var s = Stream.From(0);
            //act & assert
            for (var i = -2; i <= 2; i++)
            {
                Assert.AreEqual(Math.Max(0, i), s.Take(i).Count(), "Take should get the correct size");
            }
        }

        [TestMethod]
        public void DropTest_DropGivenAmountOfElementsFromStream()
        {
            //arrange
            var s = Stream.From(0);
            //act & assert
            for (var i = -2; i <= 2; i++)
            {
                Assert.AreEqual(Math.Max(0, i), s.Drop(i).Head, "Drop should drop the correct amount");
            }
        }

        [TestMethod]
        public void ZipWithTest_Combine2StreamsWithFunc()
        {
            //arrange 
            var t = Stream.FromThen(42, 2);
            //act
            var s = Stream.From(0).ZipWith((x, y) => x * 2 + y, Stream.Repeat(42));
            //assert
            for (var i = 0; i < 20; i++)
            {
                Assert.AreEqual(t.Head, s.Head, "ZipWith should work");
                s = s.Tail.Value;
                t = t.Tail.Value;
            }
        }

        [TestMethod]
        public void FMapTest_ReturnStreamValuesAsMapFunc()
        {
            //arrange
            var t = Stream.From(0);
            //act
            var s = Stream.FromThen(42, 2).FMap(x => (x - 42) / 2);
            //assert
            for (var i = 0; i < 20; i++)
            {
                Assert.AreEqual(t.Head, s.Head, "FMap should work");
                s = s.Tail.Value;
                t = t.Tail.Value;
            }
        }

        [TestMethod]
        public void FibTest_ReturnFirst94FibNumbers()
        {
            //arrange
            var expected = new ulong[] { 0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55,
                89, 144, 233, 377, 610, 987, 1597, 2584, 4181, 6765,
                10946, 17711, 28657, 46368, 75025, 121393, 196418, 317811,
                514229, 832040, 1346269, 2178309, 3524578, 5702887,
                9227465, 14930352, 24157817, 39088169, 63245986, 102334155,
                165580141, 267914296, 433494437, 701408733, 1134903170,
                1836311903, 2971215073, 4807526976, 7778742049, 12586269025,
                20365011074, 32951280099, 53316291173, 86267571272,
                139583862445, 225851433717, 365435296162, 591286729879,
                956722026041, 1548008755920, 2504730781961, 4052739537881,
                6557470319842, 10610209857723, 17167680177565, 27777890035288,
                44945570212853, 72723460248141, 117669030460994, 190392490709135,
                308061521170129, 498454011879264, 806515533049393,
                1304969544928657, 2111485077978050, 3416454622906707,
                5527939700884757, 8944394323791464, 14472334024676221,
                23416728348467685, 37889062373143906, 61305790721611591,
                99194853094755497, 160500643816367088, 259695496911122585,
                420196140727489673, 679891637638612258, 1100087778366101931,
                1779979416004714189, 2880067194370816120, 4660046610375530309,
                7540113804746346429, 12200160415121876738 };
            //act
            var fib = Stream.Fib();
            //assert
            for (int i = 0; i < 94; i++)
            {
                Assert.AreEqual(expected[i], fib.Head);
                fib = fib.Tail.Value;
            }
        }

        [TestMethod]
        public void PrimesTest_ReturnFirst168Primes()
        {
            //arrange
            var expected = new int[] { 2, 3, 5, 7, 11, 13,
                17, 19, 23, 29, 31, 37, 41, 43, 47, 53,
                59, 61, 67, 71, 73, 79, 83, 89, 97, 101,
                103, 107, 109, 113, 127, 131, 137, 139,
                149, 151, 157, 163, 167, 173, 179, 181,
                191, 193, 197, 199, 211, 223, 227, 229,
                233, 239, 241, 251, 257, 263, 269, 271,
                277, 281, 283, 293, 307, 311, 313, 317,
                331, 337, 347, 349, 353, 359, 367, 373,
                379, 383, 389, 397, 401, 409, 419, 421,
                431, 433, 439, 443, 449, 457, 461, 463,
                467, 479, 487, 491, 499, 503, 509, 521,
                523, 541, 547, 557, 563, 569, 571, 577,
                587, 593, 599, 601, 607, 613, 617, 619,
                631, 641, 643, 647, 653, 659, 661, 673,
                677, 683, 691, 701, 709, 719, 727, 733,
                739, 743, 751, 757, 761, 769, 773, 787,
                797, 809, 811, 821, 823, 827, 829, 839,
                853, 857, 859, 863, 877, 881, 883, 887,
                907, 911, 919, 929, 937, 941, 947, 953,
                967, 971, 977, 983, 991, 997 };
            //act
            var primes = Stream.Primes();
            //assert
            for (int i = 0; i < 168; i++)
            {
                Assert.AreEqual(expected[i], primes.Head);
                primes = primes.Tail.Value;
            }
        }
    }
}
