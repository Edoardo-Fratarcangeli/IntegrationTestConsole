namespace IntegrationTestManager.Utility;

/// <summary>
/// Extension class for <see cref="Enumerable"/>
/// </summary>
public static class EnumerableExtensions
{
    /// <summary/>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection) => collection == null || !collection.Any();

    /// <summary/>
    public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> collection) => collection?.IsNullOrEmpty() == false;

}
