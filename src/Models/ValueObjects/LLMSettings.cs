// LLMSettings.cs

using System.ComponentModel.DataAnnotations.Schema;

namespace Yggdrasil.Models.ValueObjects;

[ComplexType]
public class LLMSettings{
    /// <summary>The Model. Usually a model name, or model identifier.</summary>
    public string? Model {get; set;}
    
    // Output
    public int MaxContext {get; set;}
    public int MaxTokens {get; set;}
    public bool Streaming {get; set;} // response bit by bit
    public bool Reasoning {get; set;}
    public string? ReasoningEffort {get; set;}

    // Sampling
    public double Temperature {get; set;}
    public double TopP {get; set;}
    public int Seed {get; set;}
    public double FrequencyPenalty {get; set;}
    public double PresencePenalty {get; set;}
}
