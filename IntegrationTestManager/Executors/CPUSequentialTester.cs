using System.Diagnostics;
using IntegrationTestManager.Configuration;
using IntegrationTestManager.Utility;
using Microsoft.Extensions.Logging;

namespace IntegrationTestManager.Executors;

/// <summary>
/// Sequential tester, executed on CPU
/// </summary>
public class CPUSequentialTester : ATester, IStrategy, IExecutable
{

    #region Constructor
    public CPUSequentialTester(IContextService contextService,
                            ILogger<TestManager> logger)
        : base(contextService, logger)
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
            foreach(var test in tests)
            {
                var executionResult = ExecuteTest(test);
                Printer.PrintOutput(executionResult);
                result = result.Append(executionResult);
            }
        }
        catch (Exception ex)
        {
            AddError(ex);
        }

        return Result<IEnumerable<(Process process, string name, bool isExitedCorrectly)>>.Success(result);
    }

    public bool Match(ExecutorType executorType)
    {
        return executorType == ExecutorType.Sequential;
    }

    #endregion

}