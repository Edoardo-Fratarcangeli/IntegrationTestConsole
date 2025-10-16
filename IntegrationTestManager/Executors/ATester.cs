using System.Diagnostics;
using IntegrationTestManager.Configuration.DataServices;
using IntegrationTestManager.Utility;
using Microsoft.Extensions.Logging;

namespace IntegrationTestManager.Executors;

/// <summary>
/// Abstract class of a test executor
/// </summary>
public abstract class ATester
{
    public IContextService Context { get; init; }
    private readonly ILogger<TestManager> _logger;
    CancellationTokenSource CancellationTokenSource { get; init; }

    #region Constructor
    /// <summary>
    /// Constructor base
    /// </summary>
    public ATester(IContextService context,
                    ILogger<TestManager> logger)
    {
        Context = context;
        _logger = logger;
    }
    #endregion

    #region Abstract Methods

    /// <summary>
    /// Execute method
    /// </summary>
    public abstract void Execute();

    #endregion

    #region Protected Methods

    #region LogMessage

    protected void AddError(Exception exception = null, string message = null, params object[] args)
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
    protected void AddWarning(Exception exception = null, string message = null, params object[] args)
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
    protected void AddInfo(Exception exception = null, string message = null, params object[] args)
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

    protected static Process DecorateProcess(Process process,
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
    
	protected (Process process, string filePath, bool) ExecuteTest((string name, string commandArgument) test)
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
    #endregion
}