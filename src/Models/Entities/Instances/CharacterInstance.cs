// CharacterInstance.cs

using Yggdrasil.Models.Abstracts;

namespace Yggdrasil.Models.Entities.Instances;

public class CharacterInstance : CharacterBase{
    public required WorldInstance World { get; set; }

    /// <summary>User facing only</summary>
    public required string DisplayName;
    
    /// <summary>User facing only</summary>
    public required string Description;
}
