/// World.cs

namespace Yggdrasil.Models;

public class World{
    public Guid ID {get; set;}
    public Guid? Lorebook_ID {get; set;}
    public required string Name {get; set;}
    public required string Description {get; set;}
    public string? IntroMessage { get; set; }
    public string? NarratorInstruction {get; set;}
    public string? Scenario { get; set; }
    public List<Character> Characters {get; set;} = [];
    public List<string>? NarratorExampleDialogue {get;set;} = [];
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    public DateTime UpdatedAt {get; set;} = DateTime.UtcNow;
}
