using System.Diagnostics;
using IntegrationTestManager.Configuration.DataServices;
using Microsoft.Extensions.Logging;

namespace IntegrationTestManager.Utility;

/// <summary>
/// Class that interact with console
/// </summary>
public class ConsoleService : LogEntity<TestManager>
{
    public IContextService Context { get; init; }
	private int _incremental;
	private int _total;

    #region Constructors
    public ConsoleService(IContextService context,
                          ILogger<TestManager> logger,
                          int total)
            : base(logger, context.EnableLogger)
    {
        Context = context;
        _total = total;

        _incremental = 0;
    }
    #endregion

    #region Public Methods

    #region PrintOutput

    private void PrintOutput((Process process, string name, bool isExitedCorrectly) processAndNameAndError)
    {
        var process = processAndNameAndError.process;
        var name = processAndNameAndError.name;
        var isExitedCorrectly = processAndNameAndError.isExitedCorrectly;

        string stringResult;

        stringResult = "- " + GetPercentage() + "%";
        Write(stringResult, Blue);

        if (isExitedCorrectly == false)
        {
            stringResult = $"\tProcess Killed : ";
            Write(stringResult, Red);
        }
        else
        {
            stringResult = $"\t({process.TotalProcessorTime.Seconds} s)";
            Write(stringResult, Grey);
        }

        WriteLine("\t " + name);

        PrintVerboseOutput(process, name, isExitedCorrectly);
    }

    #endregion

    #region PrintExtremes

    private void PrintEnding(Stopwatch stopWatch) => WriteLine($"\n\nALL COMPLETED in ({stopWatch.Elapsed}) ", White);
    private void PrintTitle() => WriteLine($"\n {Context.AppName} \n", Green);

    #endregion

    #endregion

    #region Private Methods

    #region PrintVerboseOutput

    private void PrintVerboseOutput(Process process, string name, bool isExitedCorrectly)
    {
        if (Context.EnableVerbose)
        {
            if (isExitedCorrectly)
            {
                if (process.StandardOutput.ToString() is string stdOutputResult &&
                    stdOutputResult.IsNotNullOrEmpty())
                {
                    stdOutputResult = $"\t{stdOutputResult.Replace("\n", "\n\t")}";
                    WriteLine(stdOutputResult);
                }
            }
            else
            {
                if (process.StandardError.ToString() is string stdOutputError &&
                    stdOutputError.IsNotNullOrEmpty())
                {
                    stdOutputError = $"\t{stdOutputError.Replace("\n", "\n\t")}";
                    WriteLine(stdOutputError, Red);
                }
            }
        }

        AddInfo(message: $"Printed output of {name}, {(isExitedCorrectly ? "" : "NOT ")}exited correctly");
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