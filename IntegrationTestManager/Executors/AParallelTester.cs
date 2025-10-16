using IntegrationTestManager.Configuration.DataServices;
using Microsoft.Extensions.Logging;

namespace IntegrationTestManager.Executors;

/// <summary>
/// Abstract class of a parallel test executor
/// </summary>
public abstract class AParallelTester : ATester
{
    protected CancellationTokenSource CancellationTokenSource { get; init; }

    #region Constructor
    /// <summary>
    /// Constructor base
    /// </summary>
    public AParallelTester(IContextService contextService,
                            ILogger<TestManager> logger,
                            CancellationTokenSource cancellationTokenSource)
            : base(contextService, logger)
    {
        CancellationTokenSource = cancellationTokenSource;
    }
    #endregion

}