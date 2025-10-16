using IntegrationTestManager.Utility;

namespace IntegrationTestManager.Executors;

/// <summary>
/// Strategy interface for testers
/// </summary>
public interface IStrategy
{
    public bool Match(ExecutorType type);
}