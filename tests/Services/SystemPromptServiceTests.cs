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
    public void CreateNarratorPrompts_ReturnsUserSystemPrompts(){

        List<Prompt> prompts = Enumerable.Range(0, 10).Select(_ => new Prompt{Name=_faker.Lorem.Word(), Source=SourceType.FreeText, ChatPosition=ChatRole.System, Content=_faker.Lorem.Words().ToString()}).ToList();
        
        var db = Db();
        var narrator = NarratorFactory.Create(db, prompts:prompts);
        var world = WorldFactory.CreateInstance(db, narrator:narrator);

        var systemPrompt = _service.CreateNarratorPrompts(world.ID);

        var expected = string.Concat(prompts.Select(p => p.Content + Environment.NewLine));
        Assert.Equal(expected, systemPrompt);
    }
    
    [Fact]
    public void CreateNarratorPrompts_IncludesCharacters(){
        var db = Db();
        List<Prompt> prompts = [
            new Prompt{Name="Characters", Source=SourceType.Characters, ChatPosition=ChatRole.System}
        ];
        
        var narrator = NarratorFactory.Create(db, prompts:prompts);
        var world = WorldFactory.CreateInstance(db, narrator:narrator);
        var characters = Enumerable.Range(0, 2).Select(_ => CharacterFactory.CreateInstance(db, world)).ToList();

        var systemPrompt = _service.CreateNarratorPrompts(world.ID);

        foreach (var character in characters){
            var expected = new StringBuilder();
            expected.AppendLine($"<character name={character.Profile.FirstName}>");
            foreach(var property in typeof(Profile).GetProperties().OrderBy(p => p.GetCustomAttribute<OrderAttribute>()!.Value)){
                var value = property.GetValue(character.Profile);
                if(value != null) expected.AppendLine(value.ToString());
            }
            expected.AppendLine();
            expected.Append("</character>");

            Assert.Contains(expected.ToString(), systemPrompt);
        }
    }
}
