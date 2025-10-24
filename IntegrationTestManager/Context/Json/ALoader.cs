using System.Text.Json;

namespace IntegrationTestManager.Configuration;

/// <summary>
/// Json abstract loader
/// </summary>
public abstract class ALoader
{
    protected readonly JsonSerializerOptions _options;

    #region Constructor

    public ALoader(JsonSerializerOptions options = null)
    {
        _options = options ?? new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };
    }

    #endregion
}