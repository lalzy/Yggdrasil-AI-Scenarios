// Chat.cs

using Yggdrasil.Models.Enums;

namespace Yggdrasil.Models.Entities;

public class Chat{
    public Guid ID { get; set; }
    public required string Content { get; set; }
    public required Conversation Conversation { get; set; }
    public required ChatRole Role { get; set; }
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
}
