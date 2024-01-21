namespace AdventOfCode
{
    static partial class TupleUtils
    {
        public static (T a, T b) Sort<T>(this (T a, T b) range) where T : INumber<T> => range.a > range.b ? (range.b, range.a) : range;
        public static bool Contains<T>(this (T a, T b) range, T n) where T : INumber<T>
        {
            var (a, b) = range.Sort();
            return a <= n && b >= n;
        }
        public static (T, T) Ordered<T>(this (T, T) tuple) where T : IComparable<T>
        => tuple.Item1.CompareTo(tuple.Item2) >= 0
            ? tuple
            : (tuple.Item2, tuple.Item1);
        public static (T, T, T) Ordered<T>(this (T a, T b, T c) tuple) where T : IComparable<T>
        {
            var (a, b, c) = tuple;
            return (a.CompareTo(b), b.CompareTo(c), a.CompareTo(c)) switch
            {
                ( < 0, < 0, _) => (a, b, c),
                ( < 0, >= 0, < 0) => (a, c, b),
                ( < 0, >= 0, >= 0) => (c, a, b),
                ( >= 0, < 0, < 0) => (b, a, c),
                ( >= 0, < 0, >= 0) => (b, c, a),
                ( >= 0, >= 0, < 0) => (c, b, a),
                ( >= 0, >= 0, >= 0) => (c, b, a),
            };
        }

    }
}
