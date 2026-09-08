using FluentAssertions;

namespace MTG.Complete.Tests;

public static class CollectionTestExtensions
{
    public static void ShouldContainSingle<T>(
        this IEnumerable<T> collection,
        Func<T, bool> predicate,
        out T item)
    {
        item = collection.Where(predicate).Should().ContainSingle().Subject;
    }
}