// LLMSettings.cs

using System.Text.Json.Serialization;

namespace Yggdrasil.Models;

public enum APIType
{
    chatCompletion = 0
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SupportedProviders{
    LLamaCPP = 0,
    OpenRouter = 1,
}

public class LLMConnection
{
    public Guid ID { get; set; }
    public required string Name { get; set; }
    public string? APIKey  { get; set; }
    public required string URL  { get; set; }
    public string? Model  { get; set; }
    public required APIType APIType  { get; set; }
    public bool Reasoning  { get; set; }
    public required SupportedProviders Provider {get; set;}
}
