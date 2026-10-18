// Prompts.cs

using Yggdrasil.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yggdrasil.Models.ValueObjects;

[ComplexType]
public class Prompt{
    /// <summary>Human readable identifier.</summary>
    public string? Name { get; set; }
    /// <summary>Prompts content</summary>
    /// <remarks>Only used if source is user</remarks>
    public string? Content { get; set; }
    /// <summary>Source of prompt content</summary>
    public SourceType Source { get; set; }
    /// <summary>If it should be sent to the LLM or not</summary>
    public bool Active { get; set; }
}
