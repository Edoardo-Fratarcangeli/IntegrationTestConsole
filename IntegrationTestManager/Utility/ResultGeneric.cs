namespace IntegrationTestManager.Utility;

/// <summary>
/// Result object with generic content
/// </summary>
public class Result<T> : Result
{
    public T Value { get; init; }

    #region Protected Constructor

    protected Result(T value, bool succeeded)
    : base(succeeded)
    {
        Value = value;
    }

    protected Result(T value, bool succeeded, IEnumerable<Exception> exceptions)
    : base(succeeded, exceptions)
    {
        Value = value;
    }

    #endregion

    #region Generator Methods

    public static Result<T> Success(T value)
    {
        return new Result<T>(value, succeeded: true);
    }
    public static Result<T> Success(T value, IEnumerable<Exception> exceptions)
    {
        return new Result<T>(value, succeeded: true, exceptions);
    }

    public static new Result<T> Fail()
    {
        return new Result<T>(default, succeeded: false);
    }
    public static new Result<T> Fail(IEnumerable<Exception> exceptions)
    {
        return new Result<T>(default, succeeded: false, exceptions);
    }    
    public static Result<T> Fail(T value)
    {
        return new Result<T>(value, succeeded: false);
    }
    public static Result<T> Fail(T value, IEnumerable<Exception> exceptions)
    {
        return new Result<T>(value, succeeded: false, exceptions);
    }
    
    #endregion

    #region Checking Methods

    /// <summary>
    /// Invokes <paramref name="method"/> and verifies if failed
    /// </summary>
    public static bool IsFailed(Func<Result<T>> method)
    {
        if (method == null)
        {
            return true;
        }

        Result<T> result = method.Invoke();

        return result.IsFailed();
    }
    /// <summary>
    /// Invokes <paramref name="method"/> and verifies if succeded
    /// </summary>
    public static bool IsSucceeded(Func<Result<T>> method)
    {
        if (method == null)
        {
            return true;
        }

        Result<T> result = method.Invoke();

        return result.IsFailed() == false;
    }

    #endregion
}