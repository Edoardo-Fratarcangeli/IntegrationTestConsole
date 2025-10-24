
using IntegrationTestManager.Utility;
using Microsoft.Extensions.Logging;


/// <summary>
/// Class represents an object with a logger
/// </summary>
public class LogEntity<T>
{
    public readonly ILogger<T> _logger;
    
    private bool IsEnabled {get; set;}

    #region Constructor
    public LogEntity (ILogger<T> logger, bool isEnabled)
    {
        _logger = logger;
        IsEnabled = isEnabled;
    }
    #endregion

    #region Public Methods

    public void AddError(Exception exception = null, string message = null, params object[] args)
    {
        message ??= "Error";

        if (IsEnabled)
        {
            _logger.LogError(message, args);
        }

#if DEBUG
        if (exception != null)
        {
            throw new CatchedException(message, exception);
        }
#endif
    }

    public void AddWarning(Exception exception = null, string message = null, params object[] args)
    {
        message ??= "Warning";

        if (IsEnabled)
        {
            _logger.LogWarning(message, args);
        }
#if DEBUG
        if (exception != null)
        {
            throw new CatchedException(message, exception);
        }
#endif
    }

    public void AddInfo(Exception exception = null, string message = null, params object[] args)
    {
        message ??= "Information";

        if (IsEnabled)
        {
            _logger.LogInformation(message, args);
        }
#if DEBUG
        if (exception != null)
        {
            throw new CatchedException(message, exception);
        }
#endif
    }

    public ILogger<T> GetLogger()
    {
        return _logger;
    }

    #endregion
    
    
}