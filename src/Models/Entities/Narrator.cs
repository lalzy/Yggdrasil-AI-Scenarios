// Narrator.cs

using Yggdrasil.Models.Enums;

namespace Yggdrasil.Models.Entities;

public class Narrator : Agent{
    public override required LLMConnection Connection {get; set;}
}
 
