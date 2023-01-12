namespace AdventOfCode;

static class ImmutableCollectionExtensions
{
    public static IImmutableList<T> WithValueSemantics<T>(this IImmutableList<T> list) => new ImmutableListWithValueSemantics<T>(list);
}