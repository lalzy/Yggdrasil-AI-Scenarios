// World.cs

namespace Yggdrasil.Models.Entities;

public class World{
    public Guid ID { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
