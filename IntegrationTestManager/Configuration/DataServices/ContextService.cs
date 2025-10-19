using IntegrationTestManager.Utility;
using System.Linq;

namespace IntegrationTestManager.Configuration.DataServices;

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

    #endregion

    #region Private Methods

    private IEnumerable<string> CollectTests()
    {
        throw new NotImplementedException();
    }


    private string ResolveAnyAmbientVariable(string value)
    {
        throw new NotImplementedException();
    }
    #endregion
}