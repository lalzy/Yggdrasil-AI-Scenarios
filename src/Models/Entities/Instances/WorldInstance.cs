// WorldInstance.cs

using Yggdrasil.Models.Abstracts;
using Yggdrasil.Models.Entities.Templates;

namespace Yggdrasil.Models.Entities.Instances;

public class WorldInstance : WorldBase<CharacterInstance>{
    public List<Conversation> Conversations { get; set;}=[];
    
    /// <summary>When this world instance was last chatted with</sumamry>
    public DateTime LastUsed { get; set; } = DateTime.UtcNow;
}
