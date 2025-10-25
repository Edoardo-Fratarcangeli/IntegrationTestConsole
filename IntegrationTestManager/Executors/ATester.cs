using System.Diagnostics;
using IntegrationTestManager.Configuration;
using IntegrationTestManager.Utility;
using Microsoft.Extensions.Logging;

namespace IntegrationTestManager.Executors;

/// <summary>
/// Abstract class of a test executor
/// </summary>
public abstract class ATester : LogEntity<TestManager>
{
    public IContextService Context { get; init; }
    protected IPrinter Printer { get; init; }

    #region Constructor
    /// <summary>
    /// Constructor base
    /// </summary>
    public ATester(IContextService context,
                    ILogger<TestManager> logger)
            : base (logger, context.EnableLogger)
    {
        Context = context;
        Printer = new ConsolePrinter(context, logger);
    }
    #endregion

    #region Protected Methods

    protected IEnumerable<(string name, string commandArgument)> BuildTestsList()
    {
        IEnumerable<(string name, string commandArgument)> testList = [];
        
        string basePath = AppContext.BaseDirectory;
        string fullPath = Path.GetFullPath(Path.Combine(basePath, "..", "Context", "Data", "template.json"));

        if (File.Exists(fullPath) == false)
        {
            AddError(new FileNotFoundException(fullPath));
            return testList;
        }        
        if(Directory.Exists(Context.CacheFolderPath) == false)
        {
            AddError(new DirectoryNotFoundException(Context.CacheFolderPath));
            return testList;
        }

        TemplateLoader jsonLoader = new();
        if (jsonLoader.Load(fullPath) is JsonTemplateData jsonTemplate)
        {
            jsonTemplate.CacheFolderPath = Context.CacheFolderPath;
            
            foreach (var test in Context.Tests)
            {
                if (jsonTemplate.GetPersonalizedArgument(test) is string argument)
                {
                    testList = testList.Append((test, argument));
                }
            }
        }
        
        return testList;
    }

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