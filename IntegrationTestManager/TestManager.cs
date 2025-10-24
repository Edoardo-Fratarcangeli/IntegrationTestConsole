using System.Diagnostics;
using IntegrationTestManager.Configuration;
using IntegrationTestManager.Executors;
using IntegrationTestManager.Utility;
using Microsoft.Extensions.Logging;

namespace IntegrationTestManager;

/// <summary>
/// Tests executor
/// </summary>
public class TestManager : LogEntity<TestManager>
{

    private Stopwatch _stopWatch;
    
    CancellationTokenSource CancellationTokenSource { get; init; }
    IContextService Context { get; init; }
    private ExecutorType ExecutorType { get; set; }   


    #region Constructor
    /// <summary/>
    public TestManager(IContextService context,
                        ILogger<TestManager> logger,
                        CancellationTokenSource cancellationTokenSource)
            : base(logger, context.EnableLogger)
    {
        CancellationTokenSource = cancellationTokenSource;
        Context = context;
    }
    #endregion

    #region Execute

    /// <summary/>
    public Result Execute()
    {
        if (Result.IsFailed(Initialize))
        {
            return Result.Fail();
        }

        if (ExecutorFactory.Create(ExecutorType) is Result<ATester> testerResult)
        {
            if (testerResult.Succeeded)
            {
                if (testerResult.Value is IExecutable tester)
                {
                    tester.Execute();
                }
            }
            else
            {
                AddError(exception: new AggregateException(testerResult.Exceptions));
            }
        }

        if (Result.IsFailed(Finalize))
        {
            return Result.Fail();
        }

        return Result.Success();
    }

    #endregion

    #region Private Methods

    #region Initialize
    private Result Initialize()
    {
        StartTimer();
        
        ExecutorFactory.Initialize(Context, _logger, CancellationTokenSource);

        return Result.Success();
    }
    #endregion

    #region Finalize
    private Result Finalize()
    {
        EndTimer();

        return Result.Success();
    }
    #endregion

    #region Utility

	#region Timer
	private void StartTimer()
	{
		_stopWatch = new Stopwatch();
		_stopWatch.Start();
	}

    private void EndTimer()
    {
        _stopWatch.Stop();
    }
	#endregion
    
	#endregion

    #endregion
}