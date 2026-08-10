/// Conversation.cs

namespace Yggdrasil.Models;


public class Conversation{
    public Guid ID {get; set;}
    public required Guid World_ID {get; set;}
    public required string Title {get; set;}
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    public DateTime UpdatedAt {get; set;} = DateTime.UtcNow;
}
