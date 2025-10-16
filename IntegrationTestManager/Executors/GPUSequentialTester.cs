using IntegrationTestManager.Configuration.DataServices;
using IntegrationTestManager.Utility;
using Microsoft.Extensions.Logging;

namespace IntegrationTestManager.Executors;

/// <summary>
/// 
/// </summary>
public class GPUSequentialTester : ATester, IStrategy
{

    #region Constructor
    public GPUSequentialTester(IContextService contextService,
                            ILogger<TestManager> logger)
        : base(contextService, logger)
    { }
    #endregion

    #region Public Methods

    public override void Execute()
    {
        
    }

    public bool Match(ExecutorType executorType)
    {
        return executorType == ExecutorType.GPUSequential;
    }

    #endregion

}