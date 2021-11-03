using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Memory
{
    static class Memory
    {
        public static (int steps, int loopSize) Cycles(ImmutableArray<byte> input)
        {
            var list = new List<ImmutableArray<byte>>();
            var (steps, max, cycle) = (0, 0, input);                

            while (true)
            {
                var m = cycle.FindMax();
                if (m.value > max)
                    max = m.value;
                steps++;
                cycle = DoOneCycle(cycle);
                var index = list.FindIndex(x => x.SequenceEqual(cycle));
                if (index >= 0)
                {
                    return (steps, list.Count - index);
                }
                list.Add(cycle);
            };
        }

        public static (int index, int value) FindMax(this IEnumerable<byte> input)
            => input.Select((value, index) => (index: index, value: value))
                .Aggregate((index: -1, value: int.MinValue), (x, y) => y.value > x.value ? y : x);

        public static ImmutableArray<byte> DoOneCycle(ImmutableArray<byte> input)
        {
            var (max, length) = (input.FindMax(), input.Length);
            var (quotient, remainder) = (max.value / length, max.value % length);

            var query =
                from t in input.Select((value, i) => (value: value, i: i))
                let i = t.i
                let value = t.value
                let j = (i + length - max.index - 1) % length
                let term1 = i == max.index ? 0 : value
                let term2 = quotient
                let term3 = j < remainder ? 1 : 0
                select (byte)(term1+term2+term3);

            var redistributed = query.ToImmutableArray();
            return redistributed;
        }


    }
}