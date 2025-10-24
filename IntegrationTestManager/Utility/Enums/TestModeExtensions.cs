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

}

