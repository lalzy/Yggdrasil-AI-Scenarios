// Conversation.cs

using Yggdrasil.Models.Entities.Instances;
using Yggdrasil.Models.Enums;

namespace Yggdrasil.Models.Entities;

public class Conversation{
    public Guid ID { get; set; }
    public required WorldInstance World {get; set;}
    public List<Chat> Chats { get; set; }
    public required string Title { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
