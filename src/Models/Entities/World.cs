// World.cs

namespace Yggdrasil.Models.Entities;

public class World{
    public Guid ID { get; set; }

    /// <summary>Description about this world, not sent to the LLM.</summary>
    public required string Description {get; set;}

    public List<string> OpeningMessages {get; set;} = [];
    public List<Conversation>? Conversations {get; set;}

    /// <remarks>Updated when a new Chat message is made</remarks>
    public DateTime LastUsed {get; set;} = DateTime.UtcNow;
    
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Updated when World is updated</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
