/// Lorebook.cs

namespace Yggdrasil.Models;

public class Lorebook{
    public enum ActivationStates{
        constant=0,
        keyword=1,
    }
    
    public Guid ID {get; set;}
    public List<Guid> World_IDs {get; set;} = new();
    public required string Name {get;set;} 
    public ActivationStates ActivationMode {get;set;} = ActivationStates.keyword;
    public int TriggerDepth { get; set; }
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    public DateTime UpdatedAt {get; set;} = DateTime.UtcNow;
}
