// SystempromptService.cs

using Yggdrasil.Data;
using Yggdrasil.Models.Entities;
using Yggdrasil.Models.Entities.Instances;
using Yggdrasil.Models.DTO;
using Yggdrasil.Models.Enums;
using System.Linq.Dynamic.Core;
using Yggdrasil.Models.ValueObjects;
using Yggdrasil.Models.Interfaces;
using System.Text;
using System.Reflection;

namespace Yggdrasil.Services;

public class SystemPromptServices(AppDbContext db){
    private readonly DbSet<WorldInstance> _worlds = db.Set<WorldInstance>();

    /// <summary>Builds the prompt string of the character's profile values in ordered of orderedAttribute</summary>
    private string CreateCharacterString(CharacterInstance character){
        var returnString = new StringBuilder();
        foreach(var property in typeof(Profile).GetProperties().OrderBy(p => p.GetCustomAttribute<OrderAttribute>()!.Value)){
            var fieldValue = property.GetValue(character.Profile);

            if(fieldValue == null) continue;
            else{
                returnString.AppendLine(fieldValue.ToString());
            }
        }
        return returnString.ToString();
    }

    /// <summary>Buld up the system string from the agent's prompts</summary>
    private void BuildSystemString(ref StringBuilder systemPrompt, Prompt prompt, WorldInstance world, Guid? CharacterAgent=null){
        switch(prompt.Source){
            case SourceType.World:
                systemPrompt.AppendLine(world.Scenario);
                return;
            case SourceType.Characters:
                foreach(var character in world.Characters){
                    systemPrompt.AppendLine($"<character name={character.Profile.FirstName}>");
                    systemPrompt.AppendLine(CreateCharacterString(character));
                    systemPrompt.AppendLine("</character>");
                }
                return;
            // Character Agent. Will find Index of last character and append to end.
            // <self> instead of character.
            case SourceType.FreeText:
                systemPrompt.AppendLine(prompt.Content);
                return;
                
        }
        systemPrompt.Append(prompt.Content).Append('\n');
    }
    
    /// <summary>Create the system-prompt for the narrator agent.</summary>
    /// <param name="worldID">The worldID to the instance owning the narrator (current world in play)</param>
    /// <returns>The systemPrompt string</returns>
    public string CreateNarratorPrompts(Guid worldID){
        var world = _worlds.Include(w => w.Characters).Include(w => w.Narrator).First(w => w.ID == worldID);
        var systemPrompts = world.Narrator.Agent.Prompts.Where(p => p.ChatPosition == ChatRole.System).ToList();
        var systemPrompt = new StringBuilder();

        foreach(var prompt in systemPrompts){
            BuildSystemString(ref systemPrompt, prompt, world);
        }
        
        return systemPrompt.ToString();
    }
    
    /// <summary>Create the system-prompt for the character agent.</summary>
    /// <inheritdoc cref="CreateNarratorPrompts(Guid)" path="/param[@name='worldID']"/>
    /// <param name="characterID">The current character owning the prompt. Given a special highlight the LLM is 'this' character</param>
    /// <inheritdoc cref="CreateNarratorPrompts(Guid)" path="/returns"/>
    public string CreateCharacterSystemPrompt(Guid worldID, Guid characterID){
        var world = _worlds.Include(w => w.Characters).Include(w => w.Narrator).First(w => w.ID == worldID);
        return "";
    }
}
