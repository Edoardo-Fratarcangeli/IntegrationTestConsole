using IntegrationTestManager.Configuration.DataServices;
using IntegrationTestManager.Utility;
using Microsoft.Extensions.Logging;

namespace IntegrationTestManager.Executors;

/// <summary>
/// 
/// </summary>
public class CPUParallelTester : AParallelTester, IStrategy
{

    #region Constructor
    public CPUParallelTester(IContextService contextService,
                            ILogger<TestManager> logger,
                            CancellationTokenSource cancellationTokenSource)
        : base(contextService, logger, cancellationTokenSource)
    { }
    #endregion

    #region Public Methods

    public override void Execute()
    {
        
    }

    public bool Match(ExecutorType executorType)
    {
        return executorType == ExecutorType.Parallel;
    }

    #endregion

}