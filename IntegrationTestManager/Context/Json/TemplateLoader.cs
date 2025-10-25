using System.Text.Json;
using System.Text.RegularExpressions;

namespace IntegrationTestManager.Configuration;

/// <summary>
/// Json loader for template data
/// </summary>
public class TemplateLoader : ALoader, ILoader<JsonTemplateData>
{
    #region Constructor

    public TemplateLoader(JsonSerializerOptions options = null)
        : base(options)
    { }

    #endregion

    #region Public Methods

    public JsonTemplateData Load(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Config file not found: {path}");
        }

        string jsonContent = File.ReadAllText(path);
        
        Dictionary<string, string> data;
        try
        {
            data = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent)!;
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException($"Invalid JSON structure: {ex.Message}", ex);
        }

        if (data == null)
        {
            throw new InvalidDataException($"Deserialization returned null for {path}");
        }

        string template = data["commandLineArgument"];

        string result = Regex.Replace(template, @"\{\{(\w+)\}\}",
                                      match =>
                                        {
                                            string key = match.Groups[1].Value;

                                            if (data.TryGetValue(key, out string value))
                                                return value;

                                            return match.Value;
                                        });

        return new JsonTemplateData { Argument = result };
    }

    #endregion
}