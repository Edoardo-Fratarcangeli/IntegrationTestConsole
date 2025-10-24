using IntegrationTestManager.Configuration;
using IntegrationTestManager.Utility;
using Microsoft.Extensions.Logging;

namespace IntegrationTestManager.Executors;

/// <summary>
/// Sequential tester, executed on GPU
/// </summary>
public class GPUSequentialTester : ATester, IStrategy, IExecutable
{

    #region Constructor
    public GPUSequentialTester(IContextService contextService,
                            ILogger<TestManager> logger)
        : base(contextService, logger)
    { }
    #endregion

    #region Public Methods

    public Result Execute()
    {
        
    }

    public bool Match(ExecutorType executorType)
    {
        return executorType == ExecutorType.GPUSequential;
    }

    #endregion

}