using System.Text.Json;

namespace IntegrationTestManager.Configuration;

/// <summary>
/// Json loader for configuration data
/// </summary>
public class ConfigLoader : ALoader, ILoader<JsonConfigData>
{
    #region Constructor

    public ConfigLoader(JsonSerializerOptions options = null)
        : base(options)
    { }

    #endregion

    #region Public Methods

    public JsonConfigData Load(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Config file not found: {path}");
        }

        string jsonContent = File.ReadAllText(path);

        JsonConfigData data;
        try
        {
            data = JsonSerializer.Deserialize<JsonConfigData>(jsonContent, _options);
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException($"Invalid JSON structure: {ex.Message}", ex);
        }

        if (data == null)
        {
            throw new InvalidDataException($"Deserialization returned null for {path}");
        }

        Validate(data);

        return data;
    }
    #endregion


    #region Private Methods

    private static void Validate(JsonConfigData data)
    {
        if (data.Context == null)
            throw new InvalidDataException(@"Missing ""context"" section in configuration.");

        if (data.TestConfigurations == null || data.TestConfigurations.Count == 0)
            throw new InvalidDataException(@"No ""testConfigurations"" found.");

        string previousType = null;
        foreach (KeyValuePair<string, Variables> tconfigs in data.TestConfigurations)
        {
            if (string.IsNullOrWhiteSpace(tconfigs.Key))
            {
                if (previousType == null)
                    throw new InvalidDataException(@"The first test configuration has an empty ""type"".");
                else
                    throw new InvalidDataException($@"The test configuration after ""{previousType}"" has an empty ""type"".");
            }
            
            previousType = tconfigs.Key;
        }
    }

    #endregion
}