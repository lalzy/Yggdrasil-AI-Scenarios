// Chat.cs

using Yggdrasil.Models.Entities;
using Yggdrasil.Models.Enums;

namespace Yggdrasil.Models.Entities;

public class Chat{
    public Guid ID { get; set; }
    public required string Content { get; set; }
    public required Conversation Conversation { get; set; }
    public ChatRole Role { get; set; }
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
}
