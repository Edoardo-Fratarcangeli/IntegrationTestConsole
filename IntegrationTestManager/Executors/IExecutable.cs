
using IntegrationTestManager.Utility;

namespace IntegrationTestManager.Executors;

/// <summary>
/// Executing interface for any executable object
/// </summary>
public interface IExecutable
{
    /// <summary>
    /// Execute method
    /// </summary>
    public Result Execute();
}