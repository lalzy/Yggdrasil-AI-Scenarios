// SystempromptService.cs

using Yggdrasil.Data;
using Yggdrasil.Models.Entities;
using Yggdrasil.Models.Entities.Instances;
using Yggdrasil.Models.DTO;
using Yggdrasil.Models.Enums;
using System.Linq.Dynamic.Core;
using Yggdrasil.Models.ValueObjects;
using Yggdrasil.Models.Interfaces;

namespace Yggdrasil.Services;

public class SystemPromptServices(AppDbContext db){
    private readonly DbSet<WorldInstance> _worlds = db.Set<WorldInstance>();
    
    private string PopulateSystemPromptFromTemplateJson(List<Prompt> Prompts){
        return "";
    }
    
    /// <summary>Create the system-prompt for the narrator agent.</summary>
    /// <returns></returns>
    public void CreateNarratorPrompts(Guid worldID){
        var world = _worlds.Include(w => w.Characters).Include(w => w.Narrator).First(w => w.ID == worldID);
        var promptsByRole = world.Narrator.Prompts.ToLookup(p => p.ChatPosition);

        
    }
    
    /// <summary>Create the system-prompt for the character agent.</summary>
    public string CreateCharacterSystemPrompt(Guid worldID, Guid characterID){
        var world = _worlds.Include(w => w.Characters).Include(w => w.Narrator).First(w => w.ID == worldID);
        var mainCharacter = world.Characters.First(c => c.ID == characterID);
        return "";
    }
}
