// WorldInstance.cs

namespace Yggdrasil.Models.Entities;

public class WorldInstance : World{
    /// <summary>A brief summary what the RP is about sent to the LLM</summary>
    public required string Scenario {get; set;}

    /// <summary>When this world instance was last chatted with</sumamry>
    public DateTime LastUsed { get; set; } = DateTime.UtcNow;
}
