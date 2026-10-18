// SystemPrompt.cs

using Yggdrasil.Models.ValueObjects;

namespace Yggdrasil.Models.Entities;

public class SystemPrompt{
    public Guid ID { get; set; }
    public string? Name { get; set; }
    public List<Prompt> Prompts { get; set; } = new();
    public DateTime CreatedAt {get;set;} = DateTime.UtcNow;
    public DateTime UpdatedAt {get; set;} = DateTime.UtcNow;
}
