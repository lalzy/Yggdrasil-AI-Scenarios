// CharacterBase.cs

using Yggdrasil.Models.Entities;
using Yggdrasil.Models.Interfaces;
using Yggdrasil.Models.ValueObjects;

namespace Yggdrasil.Models.Abstracts;

public abstract class CharacterBase : IAgent{
    public Guid ID { get; set; }
    public required Agent Agent { get; set; }
    public LLMConnection? Connection { get; set; }
    
    /// <summary>Character's character details</summary>
    public required Profile Profile { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
