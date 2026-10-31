// PlayerBase.cs

using Yggdrasil.Models.ValueObjects;

namespace Yggdrasil.Models.Abstracts;

public abstract class PlayerBase{
    public Guid ID {get; set;}
    public required Profile Profile { get; set;}
    public required string DisplayName {get; set;}
    public string Description {get; set;}
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
