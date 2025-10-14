namespace IntegrationTestManager.Utility;

/// <summary>
/// Result object
/// </summary>
public class Result
{
    public bool Succeeded { get; init; }
    public IEnumerable<Exception> Exceptions { get; init; } = [];

    #region Protected Constructor

    protected Result(bool succeeded)
    {
        Succeeded = succeeded;
    }

    protected Result(bool succeeded, IEnumerable<Exception> exceptions)
    : this(succeeded)
    {
        Exceptions = exceptions;
    }

    #endregion

    #region Generator Methods

    public static Result Success()
    {
        return new Result(succeeded: true);
    }
    public static Result Success(IEnumerable<Exception> exceptions)
    {
        return new Result(succeeded: true, exceptions);
    }

    public static Result Fail()
    {
        return new Result(succeeded: false);
    }
    public static Result Fail(IEnumerable<Exception> exceptions)
    {
        return new Result(succeeded: false, exceptions);
    }
    
    #endregion
}