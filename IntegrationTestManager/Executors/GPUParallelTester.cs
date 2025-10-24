using IntegrationTestManager.Configuration;
using IntegrationTestManager.Utility;
using Microsoft.Extensions.Logging;

namespace IntegrationTestManager.Executors;

/// <summary>
/// Parallel tester, executed on GPU
/// </summary>
public class GPUParallelTester : AParallelTester, IStrategy, IExecutable
{

    #region Constructor
    public GPUParallelTester(IContextService contextService,
                            ILogger<TestManager> logger,
                            CancellationTokenSource cancellationTokenSource)
        : base(contextService, logger, cancellationTokenSource)
    { }
    #endregion

    #region Public Methods

    public Result Execute()
    {
        
    }

    public bool Match(ExecutorType executorType)
    {
        return executorType == ExecutorType.GPUParallel;
    }

    #endregion

}