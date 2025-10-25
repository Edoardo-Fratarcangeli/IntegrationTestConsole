using System.Collections.Concurrent;
using System.Diagnostics;
using IntegrationTestManager.Configuration;
using IntegrationTestManager.Utility;
using Microsoft.Extensions.Logging;

namespace IntegrationTestManager.Executors;

/// <summary>
/// Parallel tester, executed on CPU
/// </summary>
public class CPUParallelTester : AParallelTester, IStrategy, IExecutable
{

    #region Constructor
    public CPUParallelTester(IContextService contextService,
                            ILogger<TestManager> logger,
                            CancellationTokenSource cancellationTokenSource)
        : base(contextService, logger, cancellationTokenSource)
    { }
    #endregion

    #region Public Methods

    public Result Execute()
    {
        return ExecuteAdaptiveAsync().GetAwaiter().GetResult();
    }

    public async Task<Result> ExecuteAdaptiveAsync()
    {
        var tests = BuildTestsList().ToList();
        var results = new ConcurrentBag<(Process process, string name, bool isExitedCorrectly)>();
        try
        {
            await Parallel.ForEachAsync(
                tests,
                new ParallelOptions
                {
                    MaxDegreeOfParallelism = Context.DegreeOfParallelism?? Environment.ProcessorCount,
                    CancellationToken = CancellationTokenSource.Token
                },
                async (test, tkn) =>
                {
                    try
                    {
                        var executedTest = ExecuteTest(test);
                        Printer.PrintOutput(executedTest);
                        results.Add(executedTest);
                    }
                    catch (Exception ex)
                    {
                        AddError(ex);
                    }
                });
        }
        catch (Exception ex)
        {
            AddError(ex);
        }
        finally
        {
            CancellationTokenSource.Cancel();
        }

        return Result<IEnumerable<(Process process, string name, bool isExitedCorrectly)>>.Success(results);
    }

    public bool Match(ExecutorType executorType)
    {
        return executorType == ExecutorType.Parallel;
    }

    #endregion

}