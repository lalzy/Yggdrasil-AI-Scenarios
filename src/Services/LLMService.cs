// LLMService.cs

using Yggdrasil.Constants;
using System.Xml.Linq;
using Yggdrasil.Data;
using Yggdrasil.DTO;
using Yggdrasil.Models;
using Yggdrasil.Extensions;

namespace Yggdrasil.Services;

public class LLMService(AppDbContext db){
    private readonly AppDbContext _db = db;

    private string CreateBaseCharacterString(CharacterBase character){
        var lines = new[]{
            $"FullName: {character.FullName}",
            $"Gender: {character.Gender}",
            (character.Pronouns != null ? $"Pronouns: {character.Pronouns}" : null),
            (character.Race != null ? $"Race: {character.Race}" : null),
            (character.Occupation != null ? $"Occupation: {character.Occupation}" : null),
            (character.Appearance != null ? $"Appearance: {character.Appearance}" : null),
            (character.Equipment != null ? $"Equipment: {character.Equipment}" : null)
        };

        return string.Join("\n", lines.Where(l => l != null));
    }

    private XElement AddExampleDialogueToCharacter(Character character, XElement characterElement){
        if(character.ExampleDialogue != null){
            var exampleDialogueElement = new XElement("example-dialogue");
            foreach(var line in character.ExampleDialogue){
                exampleDialogueElement.Add(new XElement("line", line));
            }
            characterElement.Add(exampleDialogueElement);
        }
        return characterElement;
    }
    
    /// <summary>Create character XMl-like wrapping for a character</summary>
    private XElement createCharacterElement(Character character){
        var lines = new[]{
            (character.Personality != null ? $"Personality: {character.Personality}" : null),
            (character.NarrativeRole != null ? $"NarrativeRole: {character.NarrativeRole}" : null),
        };
        var characterElement = new XElement("char",
                                            new XText("\n" + string.Concat(CreateBaseCharacterString(character),
                                            "\n", string.Join("\n", lines.Where(l => l != null)))));
    
        
        return AddExampleDialogueToCharacter(character, characterElement);
    }

    private XElement CreateCharactersElement(World world){
        var charactersElement = new XElement("characters");
        charactersElement.Add(world.Characters.Select(c =>
        {
            var characterElement = createCharacterElement(c);
            characterElement.Add(new XAttribute(XNamespace.None + "name", c.Name));

            return characterElement;
        }));
        return charactersElement;
    }

    
    private XElement CreateWorldElement(World world, Persona user){
        var charactersElement = CreateCharactersElement(world);
        var userElement = new XElement("user", new XText("\n" + CreateBaseCharacterString(user)));
        userElement.Add(new XAttribute(XNamespace.None + "name", user.Name));

        
        var worldElement = new XElement("world");
        worldElement.Add(new XAttribute(XNamespace.None + "name", world.Name));
        worldElement.Add(new XText("NarratorInstruction: " + world.NarratorInstruction));
        worldElement.Add(new XText("Narrator-Example-Dialogue: " + world.NarratorExampleDialogue));

        worldElement.Add(new XElement("world", new XElement("scenario", world.Scenario, userElement, charactersElement)));
        

        return worldElement;
    }
    
    private string CreateSystemPrompt(World world, Persona persona)
    {
        var worldElement = CreateWorldElement(world, persona);
        var prompt = new XDocument(
            new XElement("system", new XElement("instruction", world.NarratorInstruction!), CreateWorldElement(world, persona)));
        return prompt.ToString(SaveOptions.None);
    }

    ///<summary>Create the message payload for LLMs</summary>
    ///<param name="world">A world object</param>
    ///<param name="persona">A persona object</param>
    ///<param name="messages">A list of message objects</param>
    ///<returns>The LLM Payload object</returns>
    public ServiceResult<LLMPayload> CreateLLMPayload(World world, Persona persona, List<Message>? messages = null){
        var payload = new LLMPayload();

        payload.Messages!.Add(new Message { Role = LLMRoles.System, Content = CreateSystemPrompt(world, persona) });
        if (world.IntroMessage != null) {
            payload.Messages.Add(new Message { Role = LLMRoles.User, Content = "" });
            payload.Messages.Add(new Message { Role = LLMRoles.Assistant, Content = world.IntroMessage });
        }
        if (messages != null) messages.ForEach(m => payload.Messages.Add(m));
        return new(payload);
    }
}
