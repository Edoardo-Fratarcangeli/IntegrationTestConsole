namespace IntegrationTestManager;

/// <summary>
/// Exception of a try/catch exception managed
/// </summary>
public class CatchedException : Exception
{
    #region Constructor
    public CatchedException()
    : base()
    { }
    public CatchedException(string message)
    : base(message)
    { }
    public CatchedException(Exception innerException)
    : base(null, innerException)
    { }
    public CatchedException(string message, Exception innerException)
    : base(message, innerException)
    { }
    #endregion
}