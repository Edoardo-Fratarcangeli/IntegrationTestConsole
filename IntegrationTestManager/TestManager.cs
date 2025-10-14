using System.Diagnostics;
using IntegrationTestManager.DataServices;
using IntegrationTestManager.Utility;
using Microsoft.Extensions.Logging;

namespace IntegrationTestManager;

/// <summary>
/// Tests executor
/// </summary>
public class TestManager
{
    IContextService Context { get; init; }

    private readonly ILogger<TestManager> _logger;
    CancellationTokenSource CancellationTokenSource { get; init; }
	private static int _incremental = 1;
	private static Stopwatch _stopWatch;

    #region Constructor
    /// <summary/>
    public TestManager(IContextService contextService,
                        ILogger<TestManager> logger,
                        CancellationTokenSource cancellationTokenSource)
    {
        Context = contextService;
        _logger = logger;
        CancellationTokenSource = cancellationTokenSource;
    }
    #endregion

    #region Execute

    /// <summary/>
    public Result Execute()
    {

		StartTimer();
        if (Initialize() is Result initializationResult &&
            initializationResult.Succeeded == false)
        {
            return initializationResult;
        }

        EndTimer();

        return Result.Success();
    }

    #endregion

    #region Private Methods

    #region LogMessage

    private void AddError(Exception exception = null, string message = null, params object[] args)
    {
        string defaultMessage = "Error";
        if(Context.EnableLogger)
            _logger.LogError(message ?? defaultMessage,  args);
#if DEBUG
        if(exception != null)
        {
            throw new CatchedException(defaultMessage, exception);
        }
#endif
    }
    private void AddWarning(Exception exception = null, string message = null, params object[] args)
    {
        string defaultMessage = "Warning";
        if(Context.EnableLogger)
            _logger.LogWarning(message ?? defaultMessage,  args);
#if DEBUG
        if(exception != null)
        {
            throw new CatchedException(defaultMessage, exception);
        }
#endif
    }
    private void AddInfo(Exception exception = null, string message = null, params object[] args)
    {
        string defaultMessage = "Information";
        if(Context.EnableLogger)
            _logger.LogInformation(message ?? defaultMessage, args);
#if DEBUG
        if(exception != null)
        {
            throw new CatchedException(defaultMessage, exception);
        }
#endif
    }

    #endregion

    #region Initialize
    private Result Initialize()
    {
        _incremental = 0;

        return Result.Success();
    }
    #endregion

	#region ExecuteTest

	private (Process process, string filePath, bool) ExecuteTest((string name, string commandArgument) test)
	{
        bool isExitedCorrectly = true;
        Process process = new();

        try
        {
            DecorateProcess(process, testExecutorPath: Context.ExePath,
                                     commandArgument: test.commandArgument);
            process.Start();
            process.WaitForExit();
        }
        catch (Exception e)
        {
            AddError(e);
            isExitedCorrectly = false;
        }
        finally
        {
            if (isExitedCorrectly == false)
            {
                process?.Kill();
            }
        }

        return (process, Path.GetFileName($"{test.name}"), isExitedCorrectly);
    }

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
    
    #region PrintOutput

    private static void PrintOutput((Process process, string name, bool isExitedCorrectly) processAndNameAndError,
                                    int total)
    {
        var process = processAndNameAndError.process;
        var name = processAndNameAndError.name;
        var isExitedCorrectly = processAndNameAndError.isExitedCorrectly;

        var stringResult = "";

        Console.ForegroundColor = ConsoleColor.Blue;
        stringResult = "- " + (int)GetPercentage(total) + "%";
        Console.Write(stringResult);

        if (isExitedCorrectly == false)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            stringResult = $"\tProcess Killed : ";
            Console.Write(stringResult);
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            stringResult = $"\t({process.TotalProcessorTime.Seconds} s)";
            Console.Write(stringResult);
        }

        Console.ResetColor();
        stringResult = "\t " + name;

        Console.WriteLine(stringResult);
    }
    
	#endregion

	#region Title/Ending
	private static void PrintEnding()
	{
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine($"\n\nALL COMPLETED in ({_stopWatch.Elapsed}) ");
		Console.ResetColor();
	}

	private static void PrintTitle()
	{
		Console.ForegroundColor = ConsoleColor.Green;

		Console.WriteLine($"\n INTEGRATION TEST CONSOLE \n");

        Console.ResetColor();
	}
	#endregion

	#region CreateShellProcess

	private static Process DecorateProcess(Process process,
                                         string testExecutorPath,
                                         string commandArgument)
    {
        process ??= new();

		process.StartInfo.FileName = testExecutorPath;
		process.StartInfo.Arguments = commandArgument;
		process.StartInfo.UseShellExecute = false;
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.RedirectStandardOutput = true;

		return process;
	}

	#endregion

    #region Utility

    #region GetPercentage
    private static double GetPercentage(double total)
    {
        var count = _incremental++;

        return count / (double)total * 100;
    }
	#endregion

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