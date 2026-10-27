// SystemPromptServiceTests.cs

using FluentAssertions;
using AutoBogus;
using Bogus;
using System.Text.Json;
using Yggdrasil.Tests.TestUtil;
using Yggdrasil.Services;
using Yggdrasil.Models.Entities;
using Yggdrasil.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Yggdrasil.Tests.Factories;
using Yggdrasil.Models.Enums.SortOrders;
using Yggdrasil.Data;
using System.Linq.Dynamic.Core;
using System.Collections;
using Yggdrasil.Models.ValueObjects;
using Yggdrasil.Models.Enums;
using System.Text;
using System.Reflection;

namespace Yggdrasil.Tests.Services;

public class SystemPromptServicesTests : DatabaseSetup{
    private readonly SystemPromptServices _service;
    private readonly Faker _faker = new();

    public SystemPromptServicesTests(){
        _service = new SystemPromptServices(Db());
    }

    [Fact]
    public void CreateNarratorSystemPrompt_ReturnsUserSystemPrompts(){

        List<Prompt> prompts = Enumerable.Range(0, 10).Select(_ => new Prompt{Name=_faker.Lorem.Word(), Source=SourceType.FreeText, ChatPosition=ChatRole.System, Content=_faker.Lorem.Words().ToString()}).ToList();
        
        var db = Db();
        var narrator = NarratorFactory.Create(db, prompts:prompts);
        var world = WorldFactory.CreateInstance(db, narrator:narrator);

        var systemPrompt = _service.CreateNarratorSystemPrompt(world.ID);

        var expected = string.Concat(prompts.Select(p => p.Content + Environment.NewLine));
        Assert.Equal(expected, systemPrompt);
    }
    
    [Fact]
    public void CreateNarratorSystemPrompt_IncludesCharacters(){
        var db = Db();
        List<Prompt> prompts = [
            new Prompt{Name="Characters", Source=SourceType.Characters, ChatPosition=ChatRole.System}
        ];
        
        var narrator = NarratorFactory.Create(db, prompts:prompts);
        var world = WorldFactory.CreateInstance(db, narrator:narrator);
        var characters = Enumerable.Range(0, 2).Select(_ => CharacterFactory.CreateInstance(db, world)).ToList();

        var systemPrompt = _service.CreateNarratorSystemPrompt(world.ID);

        foreach (var character in characters){
            var expected = new StringBuilder();
            expected.AppendLine($"<character name={character.Profile.FirstName}>");
            foreach(var property in typeof(Profile).GetProperties().OrderBy(p => p.GetCustomAttribute<OrderAttribute>()!.Value)){
                var value = property.GetValue(character.Profile);
                if(value != null) expected.AppendLine(value.ToString());
            }
            expected.Append("</character>");
            
            var start = systemPrompt.IndexOf($"<character name={character.Profile.FirstName}>");
            Assert.Equal(expected.ToString(), systemPrompt.Substring(start, expected.Length));
        }
    }

    [Fact]
    public void CreateNarratorSystemPrompt_DeactivatedPromptIsSameAsNoPrompt(){
        var db = Db();

        List<Prompt> Prompts() => [
            new Prompt{Name="Free1", Source=SourceType.FreeText, Content="<free1>", ChatPosition=ChatRole.System},
            new Prompt{Name="Free2", Source=SourceType.FreeText, Content="<free2>", ChatPosition=ChatRole.System}
        ];

        var withDeactivated = Prompts();
        withDeactivated.Insert(1, new Prompt{Name="Deactivated", Source=SourceType.FreeText, Content="<deactivated>", ChatPosition=ChatRole.System, Active=false});

        var worldWithout = WorldFactory.CreateInstance(db, narrator:NarratorFactory.Create(db, prompts:Prompts()));
        var worldWith = WorldFactory.CreateInstance(db, narrator:NarratorFactory.Create(db, prompts:withDeactivated));

        var expected = _service.CreateNarratorSystemPrompt(worldWithout.ID);
        var actual = _service.CreateNarratorSystemPrompt(worldWith.ID);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(5)]
    public void CreateCharacterSystemPrompt_ReturnsSelf(int index){
        var db = Db();
        var narrator = NarratorFactory.Create(db);
        var world = WorldFactory.CreateInstance(db, narrator:narrator);
        var characters = Enumerable.Range(0, 10).Select(_=>CharacterFactory.CreateInstance(db, world:world, prompts:[new Prompt{Name="Character", Source=SourceType.Characters, ChatPosition=ChatRole.System}])).ToList();
        var currentCharacter = characters[index];

        var systemPrompt = _service.CreateCharacterSystemPrompt(world.ID, currentCharacter.ID);

        foreach (var character in characters){
            var expected = new StringBuilder();
            var tag = (character == currentCharacter) ? "self" : "character";
            
            expected.AppendLine($"<{tag} name={character.Profile.FirstName}>");
            foreach(var property in typeof(Profile).GetProperties().OrderBy(p => p.GetCustomAttribute<OrderAttribute>()!.Value)){
                var value = property.GetValue(character.Profile);
                if(value != null) expected.AppendLine(value.ToString());
            }
            expected.Append($"</{tag}>");
            Assert.Contains(expected.ToString(), systemPrompt);
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(4)]
    public void CreateCharacterSystemPrompt_SelfIsPositionedCorrectly(int promptIndex){
        var db = Db();
        var narrator = NarratorFactory.Create(db);
        var world = WorldFactory.CreateInstance(db, narrator:narrator);

        List<Prompt> Prompts(){
            var prompts = new List<Prompt>{new Prompt{Name="Character", Source=SourceType.Characters, ChatPosition=ChatRole.System}};
            prompts.AddRange(Enumerable.Range(1, 8).Select(i => new Prompt{Name=$"Free{i}", Source=SourceType.FreeText, Content=$"<free{i}>", ChatPosition=ChatRole.System}));
            prompts.Insert(promptIndex, new Prompt{Name="CurrentCharacter", Source=SourceType.CharacterAgent, ChatPosition=ChatRole.System});
            return prompts;
        }

        var characters = Enumerable.Range(0, 5).Select(_ => CharacterFactory.CreateInstance(db, world, prompts:Prompts())).ToList();
        var currentCharacter = characters[3];
        var systemPrompt = _service.CreateCharacterSystemPrompt(world.ID, currentCharacter.ID);
        var self = systemPrompt.IndexOf("<self");

        var order = Prompts();
        for(var i = 0; i < order.Count; i++){
            if(order[i].Source != SourceType.FreeText) continue;
            var free = systemPrompt.IndexOf(order[i].Content);
            if(i < promptIndex) Assert.True(free < self);
            else Assert.True(free > self);
        }
    }

    [Fact]
    public void CreateCharacterSystemPrompt_NoCharacterAgent_SelfIsAtEndOfCharacters(){
        var db = Db();
        var narrator = NarratorFactory.Create(db);
        var world = WorldFactory.CreateInstance(db, narrator:narrator);
        var characters = Enumerable.Range(0, 10).Select(_=>CharacterFactory.CreateInstance(db, world:world, prompts:[
                               new Prompt{Name="Character", Source=SourceType.Characters, ChatPosition=ChatRole.System},
                               new Prompt{Name="Free", Source=SourceType.FreeText, Content="<free>", ChatPosition=ChatRole.System}])).ToList();
        var currentCharacter = characters[3];

        var systemPrompt = _service.CreateCharacterSystemPrompt(world.ID, currentCharacter.ID);
        var self = systemPrompt.IndexOf("<self");

        Assert.True(self > systemPrompt.LastIndexOf("<character name="));
        Assert.True(self < systemPrompt.IndexOf("<free>"));
    }
    
    [Fact]
    public void CreateCharacterSystemPrompt_DeactivatedPromptIsSameAsNoPrompt(){
        var db = Db();

        List<Prompt> Prompts() => [
            new Prompt{Name="Free1", Source=SourceType.FreeText, Content="<free1>", ChatPosition=ChatRole.System},
            new Prompt{Name="Free2", Source=SourceType.FreeText, Content="<free2>", ChatPosition=ChatRole.System}
        ];

        var withDeactivated = Prompts();
        withDeactivated.Insert(1, new Prompt{Name="Deactivated", Source=SourceType.FreeText, Content="<deactivated>", ChatPosition=ChatRole.System, Active=false});

        var worldWithout = WorldFactory.CreateInstance(db, narrator:NarratorFactory.Create(db));
        var characterWithout = CharacterFactory.CreateInstance(db, worldWithout, prompts:Prompts());

        var worldWith = WorldFactory.CreateInstance(db, narrator:NarratorFactory.Create(db));
        var characterWith = CharacterFactory.CreateInstance(db, worldWith, prompts:withDeactivated);

        var expected = _service.CreateCharacterSystemPrompt(worldWithout.ID, characterWithout.ID);
        var actual = _service.CreateCharacterSystemPrompt(worldWith.ID, characterWith.ID);

        Assert.Equal(expected, actual);
    }
}
