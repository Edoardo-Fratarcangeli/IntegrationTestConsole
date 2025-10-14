namespace IntegrationTestManager;

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
            1 => TestMode.Sequential,
            2 => TestMode.Parallel,
            _ => TestMode.None
        };
    }

    /// <summary/>
    public static bool AsParallel(this TestMode mode)
    {
        return mode == TestMode.Parallel;
    }
}

