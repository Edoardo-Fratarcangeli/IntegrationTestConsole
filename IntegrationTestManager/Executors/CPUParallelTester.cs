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
        IEnumerable<(string name, string commandArgument)> tests = BuildTestsList();

        IEnumerable<(Process, string, bool)> result = null;
        try
        {
            object lockObj = new();
            int total = tests.Count();

            result = tests.AsParallel()
                            .WithDegreeOfParallelism(Context.DegreeOfParallelism?? Environment.ProcessorCount)
                            .WithCancellation(CancellationTokenSource.Token)
                            .Select(ExecuteTest)
                            .Select(executedTest =>
                            {
                                lock (lockObj)
                                {
                                    Printer.PrintOutput(executedTest);
                                }
                                return executedTest;
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

        return Result<IEnumerable<(Process process, string name, bool isExitedCorrectly)>>.Success(result);
    }

    public bool Match(ExecutorType executorType)
    {
        return executorType == ExecutorType.Parallel;
    }

    #endregion

}