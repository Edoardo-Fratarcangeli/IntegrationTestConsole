namespace IntegrationTestManager.Utility;

/// <summary>
/// Exception of a failed result
/// </summary>
public class ResultFailException : Exception
{
    #region Constructor
    public ResultFailException()
    : base()
    { }
    public ResultFailException(string message)
    : base(message)
    { }
    public ResultFailException(Exception innerException)
    : base(null, innerException)
    { }
    public ResultFailException(string message, Exception innerException)
    : base(message, innerException)
    { }
    #endregion
}