// Agent.cs

using System.ComponentModel.DataAnnotations.Schema;

namespace Yggdrasil.Models.ValueObjects;

[ComplexType]
public class Agent{
    public Guid ID {get; set;}

    /// <summary>Display name</summary>
    public required string DisplayName {get; set;}

    /// <summary>Description about the Agent. User facing only</summary>
    public required string Description { get; set; }

    public required List<Prompt> Prompts {get; set;} = [];
    
    /// <summary>The agent's LLM Connection. If null for characters will use narrator's Connection.</summary>
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    public DateTime UpdatedAt {get; set;} = DateTime.UtcNow;
}
