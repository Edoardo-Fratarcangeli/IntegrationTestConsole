namespace IntegrationTestManager.Utility;

/// <summary>
/// Result extensions
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Verifies if any exceptions
    /// </summary>
    public static bool? AnyError<T>(this Result<T> result)
    {
        if (result == null)
        {
            return null;
        }

        return result.Exceptions.Any();
    }

    /// <summary>
    /// Verifies if any exceptions
    /// </summary>
    public static bool? AnyError(this Result result)
    {
        if (result == null)
        {
            return null;
        }

        return result.Exceptions.Any();
    }

    /// <summary>
    /// Verifies if failed
    /// </summary>
    public static bool IsFailed(this Result result)
    {
        if (result == null)
        {
            return true;
        }

        return result.Succeeded == false;
    }
    /// <summary>
    /// Verifies if failed and collects itself
    /// </summary>
    public static bool IsFailed(this Result result, out Result outResult)
    {
        outResult = result;

        return result.IsFailed();
    }

    /// <summary>
    /// Verifies if failed
    /// </summary>
    public static bool IsFailed<T>(this Result<T> result)
    {
        if (result == null)
        {
            return true;
        }

        return result.Succeeded == false;
    }
    /// <summary>
    /// Verifies if failed and collects itself
    /// </summary>
    public static bool IsFailed<T>(this Result<T> result, out Result<T> outResult)
    {
        outResult = result;

        return result.IsFailed<T>();
    }
}