// Character.cs

using Yggdrasil.Models.ValueObjects;
using Yggdrasil.Models.Enums;

namespace Yggdrasil.Models.Entities.Templates;

public class CharacterTemplate{
    public Guid ID { get; set; }
    public required Agent Agent {get; set;}
    public LLMConnection? Connection {get; set;    }
    /// <summary>Character's character details</summary>
    public required Profile Profile {get; set;}
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt { get; set; }
}
