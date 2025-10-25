namespace IntegrationTestManager.Utility;

/// <summary>
/// Extension class for <see cref="Enumerable"/>
/// </summary>
public static class StringExtensions
{
    /// <summary/>
    public static bool IsNullOrEmpty(this string @string) => @string == null || string.IsNullOrWhiteSpace(@string);

    /// <summary/>
    public static bool IsNotNullOrEmpty(this string @string) => @string.IsNullOrEmpty() == false;

}
