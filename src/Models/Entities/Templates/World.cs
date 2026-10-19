// World.cs

namespace Yggdrasil.Models.Entities.Templates;

public class WorldTemplate{
    public Guid ID { get; set; }

    public required string Name {get; set;}
    
    /// <summary>Description about this world, not sent to the LLM.</summary>
    public required string Description {get; set;}
    
    /// <summary>A brief summary what the RP is about sent to the LLM</summary>
    public required string Scenario {get; set;}
    
    /// <summary>Current Narrator Agent</summary>
    public required Narrator Narrator {get; set;}

    public List<string> OpeningMessages {get; set;} = [];
    public List<string> Tags {get; set;} = [];
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
