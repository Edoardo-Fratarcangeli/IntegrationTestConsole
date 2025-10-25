namespace IntegrationTestManager.Utility;

/// <summary>
/// Exception of a json configuration not found in config.json
/// </summary>
public class JsonConfigDataNotFoundException : Exception
{
    #region Constructor
    public JsonConfigDataNotFoundException()
    : base()
    { }
    public JsonConfigDataNotFoundException(string message)
    : base($"{message} configutation not found")
    { }
    #endregion
}