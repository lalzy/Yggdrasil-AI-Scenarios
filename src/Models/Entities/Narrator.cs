// Narrator.cs

using Yggdrasil.Models.Interfaces;
using Yggdrasil.Models.ValueObjects;

namespace Yggdrasil.Models.Entities; 

public class Narrator : IAgent{
    public Guid ID {get; set;}
    public required Agent Agent {get; set;}
    public required LLMConnection Connection {get; set;}
    
    public DateTime CreatedAt {get; set;}
    public DateTime UpdatedAt { get; set; }
}
 
