// LLMConnection.cs

namespace Yggdrasil.Models.Entities;

public class LLMConnection{
    public Guid ID {get; set;}

    /// <sumamry>Identifier for the LLMCOnnection listing</summary>
    public required string Name {get; set;}

    /// <summary>URL to the provider</summary>
    public required string URL {get; set;}

    /// <summary>Port number to the LLM if provider uses a dedicated port.</summary>
    public int? port {get;set;}
    
    /// <summary>The Model. Usually a model name, or model identifier.</summary>
    public string? Model {get; set;}

    /// <summary>Json request format, must match the provider used</summary>
    public required string RequestFormat {get; set;}
    
    /// <summary>Json response format, must match the provider used</summary>
    public required string ResponseFormat {get; set;}
    
    /// <summary>Request Headers the provider requires</summary>
    public string? Headers {get;set;}
    
    /// <summary>The Authorization key</summary>
    public string? APIKey {get; set;}

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
