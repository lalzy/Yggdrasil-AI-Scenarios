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

    private string createCharacterString(Character character){
        var lines = new[]{
            (character.Personality != null ? $"Personality: {character.Personality}" : null),
            (character.NarrativeRole != null ? $"NarrativeRole: {character.NarrativeRole}" : null),
        };
        
        return string.Concat(CreateBaseCharacterString(character), "\n", string.Join("\n", lines.Where(l => l != null)));
    }

    private string CreateSystemPrompt(World world, Persona persona)
    {
        var userElement = new XElement("user", new XText("\n" + CreateBaseCharacterString(persona)));
        userElement.Add(new XAttribute(XNamespace.None + "name", persona.Name));
        var charactersElement = new XElement("characters");

        // Create <char name="name"> for each character
        // wrapped in <characters> tag
        charactersElement.Add(world.Characters.Select(c =>
        {
            var characterElement = new XElement("char", new XText("\n" + createCharacterString(c)));
            characterElement.Add(new XAttribute(XNamespace.None + "name", c.Name));

            if(c.ExampleDialogue != null){
                var exampleDialogueElement = new XElement("example-dialogue");
                foreach(var line in c.ExampleDialogue){
                    exampleDialogueElement.Add(new XElement("line", line));
                }
            characterElement.Add(exampleDialogueElement);
        }
            
            return characterElement;
        }));

        var prompt = new XDocument(
            new XElement("system", new XElement("instruction", world.NarratorInstruction!),
                new XElement("world",
                    new XElement("scenario", world.Scenario),
                        userElement, charactersElement)));
        return prompt.ToString(SaveOptions.None);
    }

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
