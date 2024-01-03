namespace AdventOfCode;

static class ImmutableCollectionExtensions
{
    public static ImmutableListWithValueSemantics<T> WithValueSemantics<T>(this IImmutableList<T> list) => new ImmutableListWithValueSemantics<T>(list);
}