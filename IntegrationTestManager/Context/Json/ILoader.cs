namespace IntegrationTestManager.Configuration;

/// <summary>
/// Interface to a .json loader
/// </summary>
public interface ILoader<T>
{
    /// <summary>
    /// Loader method
    /// </summary>
    T Load(string filePath);
}