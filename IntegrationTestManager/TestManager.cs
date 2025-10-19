using System.Diagnostics;
using IntegrationTestManager.Configuration.DataServices;
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
                testerResult.Value.Execute();
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

	#region ExecuteTest

    private IEnumerable<(Process process, string name, bool isExitedCorrectly)> ExecuteTests(IEnumerable<(string name, string commandArgument)> tests)
    {
        IEnumerable<(Process, string, bool)> result = null;
        try
        {
            object lockObj = new();
            int total = tests.Count();

            if(Context.TestMode.AsParallel())
            result = tests.AsParallel()
                            .WithDegreeOfParallelism(Context.DegreeOfParallelism?? Environment.ProcessorCount)
                            .WithCancellation(CancellationTokenSource.Token)
                            .Select(ExecuteTest)
                            .Select(process =>
                            {
                                lock (lockObj)
                                {
                                    PrintOutput(process, total);
                                }
                                return process;
                            })
                            .ToList();
        }
        catch (Exception ex)
        {
            AddError(ex);
        }
        finally
        {
            CancellationTokenSource.Cancel();
        }

        return result;
    }

	#endregion

    #region Utility

	#region Timer
	private static void StartTimer()
	{
		_stopWatch = new Stopwatch();
		_stopWatch.Start();
	}

    private static void EndTimer()
    {
        _stopWatch.Stop();
    }
	#endregion
    
	#endregion

    #endregion
}