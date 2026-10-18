// Character.cs

using Yggdrasil.Models.ValueObjects;
using Yggdrasil.Models.Enums;

namespace Yggdrasil.Models.Entities.Templates;

public class CharacterTemplate : Agent{
    /// <summary>Character's character details</summary>
    public required Profile Profile {get; set;}
}
