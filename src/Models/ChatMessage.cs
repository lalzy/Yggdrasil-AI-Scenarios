/// ChatMessage.cs

namespace Yggdrasil.Models;

public enum RoleType{
    system=0,
    assistant=1,
    user=2,
}

public class ChatMessage{
    public Guid ID {get; set;}
    public required Guid Conversation_ID {get; set;}
    public required RoleType Role {get; set;}
    public required string Content {get; set;}
    public DateTime TimeStamp {get; set;} = DateTime.UtcNow;

}
