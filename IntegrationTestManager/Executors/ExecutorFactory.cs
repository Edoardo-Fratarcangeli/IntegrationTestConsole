using System.Reflection;
using IntegrationTestManager.Utility;

namespace IntegrationTestManager.Executors;

/// <summary>
/// Factory that creates the specific tester
/// </summary>
public class ExecutorFactory
{
    private readonly List<Type> Registry;

    #region Constructor
    private ExecutorFactory()
    {
        Registry = [.. Assembly.GetExecutingAssembly()
                                .GetTypes()
                                .Where(t => !t.IsAbstract)
                                .Where(t => t.IsSubclassOf(typeof(ATester)))];
    }
    #endregion

    #region Create
    public static ATester Create(ExecutorType executorType)
    {
        ExecutorFactory factory = new();

        foreach (var type in factory.GetRegistry())
        {
            if (Activator.CreateInstance(type) is IStrategy strategy)
            {
                if (strategy.Match(executorType))
                {
                    return (ATester)strategy;
                }
            }
        }

        return null;
    }
    #endregion
    
    #region Private Methods
    private List<Type> GetRegistry()
    {
        return Registry;
    }
    #endregion
}