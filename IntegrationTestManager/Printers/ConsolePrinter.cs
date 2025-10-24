using System.Diagnostics;
using IntegrationTestManager.Configuration;
using IntegrationTestManager.Executors;
using Microsoft.Extensions.Logging;

namespace IntegrationTestManager.Utility;

/// <summary>
/// Class that prints on the console
/// </summary>
public class ConsolePrinter : LogEntity<TestManager>, IPrinter
{
    public IContextService Context { get; init; }
	private int _incremental;
	private readonly int _total;

    #region Constructors
    public ConsolePrinter(IContextService context,
                          ILogger<TestManager> logger)
            : base(logger, context.EnableLogger)
    {
        Context = context;
        _total = context.Tests.Count();
        _incremental = 0;
    }
    #endregion

    #region Public Methods

    #region PrintOutput

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void PrintOutput((Process process, string name, bool isExitedCorrectly) test)
    {
        string stringResult;

        stringResult = "- " + GetPercentage() + "%";
        Write(stringResult, Blue);

        if (test.isExitedCorrectly == false)
        {
            stringResult = $"\tProcess Killed : ";
            Write(stringResult, Red);
        }
        else
        {
            stringResult = $"\t({test.process.TotalProcessorTime.Seconds} s)";
            Write(stringResult, Grey);
        }

        WriteLine("\t " + test.name);

        PrintVerboseOutput(test);
    }

    #endregion

    #region PrintExtremes

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void PrintEnding(Stopwatch stopWatch) => WriteLine($"\n\nALL COMPLETED in ({stopWatch.Elapsed}) ", White);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void PrintTitle() => WriteLine($"\n {Context.AppName} \n", Green);

    #endregion

    #endregion

    #region Private Methods

    #region PrintVerboseOutput

    private void PrintVerboseOutput((Process process, string name, bool isExitedCorrectly) test)
    {
        if (Context.EnableVerbose)
        {
            if (test.isExitedCorrectly)
            {
                if (test.process.StandardOutput.ToString() is string stdOutputResult &&
                    stdOutputResult.IsNotNullOrEmpty())
                {
                    stdOutputResult = $"\t{stdOutputResult.Replace("\n", "\n\t")}";
                    WriteLine(stdOutputResult);
                }
            }
            else
            {
                if (test.process.StandardError.ToString() is string stdOutputError &&
                    stdOutputError.IsNotNullOrEmpty())
                {
                    stdOutputError = $"\t{stdOutputError.Replace("\n", "\n\t")}";
                    WriteLine(stdOutputError, Red);
                }
            }
        }

        AddInfo(message: $"Printed output of {test.name}, {(test.isExitedCorrectly ? "" : "NOT ")}exited correctly");
    }
    
    #endregion 
    
    #region GetPercentage

    private int GetPercentage()
    {
        return (int)(_incremental++ / (double)_total * 100);
    }

    #endregion

    #region Colors

    private static void Red() => Console.ForegroundColor = ConsoleColor.Red;
    private static void Green() => Console.ForegroundColor = ConsoleColor.Green;
    private static void Blue() => Console.ForegroundColor = ConsoleColor.Blue;
    private static void Grey() => Console.ForegroundColor = ConsoleColor.DarkGray;
    private static void White() => Console.ForegroundColor = ConsoleColor.White;
    private static void Default() => Console.ResetColor();

    #endregion

    #region Console

    private static void Write(string @string) => Console.Write(@string);
    private static void Write(string @string, Action colorize)
    {
        colorize.Invoke();
        Console.Write(@string);
        Default();
    }

    private static void WriteLine(string @string) => Console.WriteLine(@string);
    private static void WriteLine(string @string, Action colorize)
    {
        colorize.Invoke();
        Console.WriteLine(@string);
        Default();
    }

    #endregion

    #endregion
}