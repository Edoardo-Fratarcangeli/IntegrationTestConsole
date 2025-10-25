namespace IntegrationTestManager.Utility;

/// <summary>
/// Class for extension methods of <see cref="TestMode"/>
/// </summary>
public static class TestModeExtensions
{
    /// <summary/>
    public static TestMode ToTestMode(this ushort? value)
    {
        return value switch
        {
            1 => TestMode.Parallel,
            2 => TestMode.Sequential,
            _ => TestMode.None
        };
    }

    /// <summary/>
    public static TestMode ToTestMode(this string value)
    {
        if (value.IsNullOrEmpty())
        {
            return TestMode.None;
        }

        if (int.TryParse(value, out int numeric))
        {
            return numeric switch
            {
                1 => TestMode.Parallel,
                2 => TestMode.Sequential,
                _ => TestMode.None
            };
        }

        if (Enum.TryParse<TestMode>(value, true, out var result))
            return result;
        
        return value.ToUpperInvariant() switch
        {
            "P" => TestMode.Parallel,
            "S" => TestMode.Sequential,
            _ => TestMode.None
        };
    }

}

