// Agent.cs

namespace Yggdrasil.Models.Entities;

public class Agent{
    public Guid ID {get; set;}

    /// <summary>Display name</summary>
    public required string DisplayName {get; set;}

    /// <summary>Description about the Agent. User facing only</summary>
    public required string Description { get; set; }
    
    /// <summary>Unique instruction to the Agent added to the Prompt-Generator</summary>
    public required string Instruction {get; set;}

    /// <summary>The agent's LLM Connection. If null for characters will use narrator's Connection.</summary>
    public virtual LLMConnection? Connection {get; set;}
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    public DateTime UpdatedAt {get; set;} = DateTime.UtcNow;
}
