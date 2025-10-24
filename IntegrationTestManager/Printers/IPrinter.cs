using System.Diagnostics;
namespace IntegrationTestManager.Utility;

/// <summary>
/// Printing interface for showing results
/// </summary>
public interface IPrinter
{
    /// <summary>
    /// Printing method of a specific test in runtime
    /// </summary>
    public void PrintOutput((Process process, string name, bool isExitedCorrectly) test);

    /// <summary/>
    public void PrintEnding(Stopwatch stopWatch);

    /// <summary/>
    public void PrintTitle();
}