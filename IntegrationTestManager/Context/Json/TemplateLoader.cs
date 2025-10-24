using System.Text.Json;
using System.Text.RegularExpressions;

namespace IntegrationTestManager.Configuration;

/// <summary>
/// Json loader for template data
/// </summary>
public class TemplateLoader : ALoader, ILoader<string>
{
    #region Constructor

    public TemplateLoader(JsonSerializerOptions options = null)
        : base(options)
    { }

    #endregion

    #region Public Methods

    public string Load(string path)
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
                                            return data.TryGetValue(key, out string value) ? value : match.Value;
                                        });

        return result;
    }

    #endregion
}