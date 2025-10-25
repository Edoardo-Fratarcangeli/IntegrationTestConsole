using IntegrationTestManager.Utility;

namespace IntegrationTestManager.Configuration;

/// <summary>
/// Context service interface 
/// </summary>
public interface IContextService
{
    #region Public Properties

    /// <summary/>
    public string AppName { get; }
    /// <summary/>
    public string CacheFolderPath { get; }
    /// <summary/>
    public int? DegreeOfParallelism { get; }
    /// <summary/>
    public bool EnableLogger { get; }
    /// <summary/>
    public bool EnableVerbose { get; }
    /// <summary/>
    public string ExePath { get; }
    /// <summary/>
    public bool IsValid { get; }
    /// <summary/>
    public TestMode TestMode { get; }
    /// <summary/>
    public IEnumerable<string> Tests { get; }

    #endregion

    #region Public Methods

    #region Setters

    /// <summary/>
    public IContextService SetAppName(string value);
    /// <summary/>
    public IContextService SetArgs(string[] value);
    /// <summary/>
    public IContextService SetCacheFolderPath(string value);
    /// <summary/>
    public IContextService SetDegreeOfParallelism(int? value);
    /// <summary/>
    public IContextService SetEnableLogger(bool value);
    /// <summary/>
    public IContextService SetEnableVerbose(bool value);
    /// <summary/>
    public IContextService SetExePath(string value);
    /// <summary/>
    public IContextService SetTestMode(TestMode value);
    /// <summary/>
    public IContextService SetTests(IEnumerable<string> value);

    #endregion

    /// <summary>
    /// Set inner properties from Main args or Json
    /// </summary>
    /// <returns></returns>
    Result SetProperties();

    #endregion
}