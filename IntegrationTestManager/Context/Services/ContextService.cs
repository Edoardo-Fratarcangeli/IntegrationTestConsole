using IntegrationTestManager.Utility;
using System.Linq;
using System.Reflection;

namespace IntegrationTestManager.Configuration;

/// <summary>
/// Context service to set app all the necessities of the application
/// </summary>
public class ContextService : IContextService
{

    #region Public Properties

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string AppName { get; private set; }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    private string[] Args { get; set; }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string CacheFolderPath { get; private set; }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int? DegreeOfParallelism { get; private set; }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool EnableLogger { get; private set; }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool EnableVerbose { get; private set; }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string ExePath { get; private set; }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsValid { get; private set; }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool UseGPUComputation { get; private set; }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public TestMode TestMode { get; private set; }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerable<string> Tests { get; private set; }

    #endregion

    /// <summary>
    /// Constructor
    /// </summary>
    public ContextService()
    { }

    #region Public Methods

    #region Setters
    
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IContextService SetAppName(string value)
    {
        AppName = value;
        return this;
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IContextService SetArgs(string[] value)
    {
        Args = value;
        return this;
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IContextService SetCacheFolderPath(string value)
    {
        CacheFolderPath = value;
        return this;
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IContextService SetDegreeOfParallelism(int? value)
    {
        DegreeOfParallelism = value;
        return this;
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IContextService SetEnableLogger(bool value)
    {
        EnableLogger = value;
        return this;
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IContextService SetEnableVerbose(bool value)
    {
        EnableVerbose = value;
        return this;
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IContextService SetExePath(string value)
    {
        ExePath = ResolveAnyAmbientVariable(value);
        return this;
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IContextService SetUseGPUComputation(bool value)
    {
        UseGPUComputation = value;
        return this;
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IContextService SetTestMode(TestMode value)
    {
        TestMode = value;
        return this;
    }
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IContextService SetTests(IEnumerable<string> value)
    {
        Tests = value;
        return this;
    }
    
    #endregion

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerable<string> GetTestPaths()
    {
        if (Tests.IsNotNullOrEmpty())
            return Tests;
        else
            return CollectTests() ?? [];
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Result SetProperties()
    {
        try
        {
            SetAppName(Assembly.GetEntryAssembly().GetName().Name);

            if (Args.Length != 0)
            {
                // --- set properties from command line if specified ---
                if (CollectFromCommandLine().IsFailed())
                {
#if DEBUG
                    throw new ResultFailException(message: $"Collected in {nameof(CollectFromCommandLine)}");
#else
                    Console.WriteLine($"{nameof(ResultFailException)} [{nameof(CollectFromCommandLine)}]");
                    return Result.Fail();
#endif
                }
            }
            else
            {
                // --- set properties from config.json ---
                if (CollectFromJason().IsFailed())
                {
#if DEBUG
                    throw new ResultFailException(message: $"Collected in {nameof(CollectFromJason)}");
#else
                Console.WriteLine($"{nameof(ResultFailException)} [{nameof(CollectFromJason)}]");
                return Result.Fail();
#endif
                }
            }

        }
        catch (Exception e)
        {
#if DEBUG
            throw new CatchedException(message: $"Collected in {nameof(SetProperties)}", innerException: e);
#else
			Console.WriteLine($"{nameof(CatchedException)} [{nameof(SetProperties)}] : {e}");
            return Result.Fail();
#endif
        }

        IsValid = true;
        return Result.Success();
    }
    #endregion

    #region Private Methods

	private Result CollectFromJason()
    {
        string basePath = AppContext.BaseDirectory;
        string fullPath = Path.GetFullPath(Path.Combine(basePath, "..", "Data", "config.json"));

        ConfigLoader jsonLoader = new();
        if(jsonLoader.Load(fullPath) is JsonConfigData jsonConfig)
        {
            if(jsonConfig.GetCurrentVariables() is Variables variables)
            {
                this.SetCacheFolderPath(variables.CacheFolderPath)
                    .SetDegreeOfParallelism(variables.DegreeOfParallelism?? Environment.ProcessorCount)
                    .SetEnableLogger(variables.EnableLogger ?? false)
                    .SetEnableVerbose(variables.EnableVerbose ?? false)
                    .SetExePath(variables.ExePath)
                    .SetUseGPUComputation(variables.UseGPUComputation ?? false)
                    .SetTestMode(variables.TestMode.ToTestMode())
                    .SetTests(variables.Tests);

                return Result.Success();
            }
        }

        return Result.Fail();
	}
	
    private Result CollectFromCommandLine()
    {
		CommandLineParser parser = new();
		parser.Parse(Args);

        if (parser.Options is CommandLineOptions options)
        {
            this.SetCacheFolderPath(options.CacheFolderPath)
                .SetDegreeOfParallelism(options.DegreeOfParallelism?? Environment.ProcessorCount)
                .SetEnableLogger(options.EnableLogger ?? false)
                .SetEnableVerbose(options.EnableVerbose ?? false)
                .SetExePath(options.ExePath)
                .SetUseGPUComputation(options.UseGPUComputation ?? false)
                .SetTestMode(options.TestMode.ToTestMode())
                .SetTests(options.Tests);

            return Result.Success();
        }

        return Result.Fail();
    }

    private List<string> CollectTests()
    {
        List<string> fullPathTests = [];

        if (Tests.IsNotNullOrEmpty())
        {
            foreach (var test in Tests)
            {
                fullPathTests.Add(Path.Combine(CacheFolderPath));
            }
        }
        else
        {
            

        }

        return fullPathTests;
    }

    private string ResolveAnyAmbientVariable(string value)
    {
        throw new NotImplementedException();
    }
    #endregion
}