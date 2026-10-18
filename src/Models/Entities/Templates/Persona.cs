// Persona.cs

using Yggdrasil.Models.ValueObjects;

namespace Yggdrasil.Models.Entities.Templates;

public class PersonaTemplate{
    public Guid ID {get; set;}
    /// <summary>User facing only</summary>
    public required string DisplayName;
    
    /// <summary>User facing only</summary>
    public required string Description;
    
    /// <summary>Persona's character details</summary>
    public required Profile Profile;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
