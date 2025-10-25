using System.Text.Json.Serialization;
using IntegrationTestManager.Utility;

namespace IntegrationTestManager.Configuration;

/// <summary>
/// Template block
/// </summary>
public record class JsonTemplateData
{
    public string Argument { get; init; } = string.Empty;
    public string CacheFolderPath { get; set; } = string.Empty;

    /// <summary>
    /// Get the personalized command line argument
    /// </summary>
    /// <returns></returns>
    public string GetPersonalizedArgument(string testName)
    {
        if (Argument.IsNotNullOrEmpty() && CacheFolderPath.IsNotNullOrEmpty())
        {
            if(testName.IsNotNullOrEmpty())
            {
                return CacheFolderPath + Argument.Replace("{{testName}}", $"{testName}");
            }
        }

        return null;
    }
}

/// <summary>
/// Configuration root block
/// </summary>
public record class JsonConfigData
{
    [JsonPropertyName("context")]
    public Context Context { get; init; } = new();

    [JsonPropertyName("testConfigurations")]
    public Dictionary<string, Variables> TestConfigurations { get; init; } = [];

    /// <summary>
    /// Get the chosen variables set
    /// </summary>
    /// <returns></returns>
    public Variables GetCurrentVariables()
    {
        if (Context.UseConfigJson)
        {
            if(TestConfigurations.TryGetValue(Context.TestConfigurationChosen, out Variables variables))
            {
                return variables;
            }
        }

        return null;
    }
}

/// <summary>
/// Context block
/// </summary>
public record class Context
{
    [JsonPropertyName("useConfigJson")]
    public bool UseConfigJson { get; init; }

    [JsonPropertyName("testConfigurationChosen")]
    public string TestConfigurationChosen { get; init; } = string.Empty;
}

/// <summary>
/// Variables cho
/// </summary>
public record class Variables
{
    [JsonPropertyName("CacheFolderPath")]
    public string CacheFolderPath { get; init; } = string.Empty;

    [JsonPropertyName("DegreeOfParallelism")]
    public int? DegreeOfParallelism { get; init; }

    [JsonPropertyName("EnableLogger")]
    public bool? EnableLogger { get; init; }

    [JsonPropertyName("EnableVerbose")]
    public bool? EnableVerbose { get; init; }

    [JsonPropertyName("ExePath")]
    public string ExePath { get; init; } = string.Empty;

    [JsonPropertyName("TestMode")]
    public ushort? TestMode { get; init; }

    [JsonPropertyName("Tests")]
    public IEnumerable<string> Tests { get; init; } = [];
}