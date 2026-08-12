// LLMServiceTests.cs

using System.Reflection;
using Yggdrasil.Tests.Factories;
using Yggdrasil.Services;
using System.Xml.Linq;
using Yggdrasil.DTO;
using Yggdrasil.Models;
using Yggdrasil.Constants;
using Yggdrasil.Extensions;

using Xunit.Abstractions;

namespace Yggdrasil.Tests.Services;

public class LLMServiceTests : DatabaseTestBase{
    private readonly LLMService _service;
    private readonly Faker _faker = new();

    public LLMServiceTests(DatabaseFixture fixture) :base(fixture){
        _service = new LLMService(fixture.CreateContext());
    }

    // Helpers
    private (World, Character, Persona) CreateData(){
        var world = WorldFactory.Create(_fixture);
        var character = CharacterFactory.Create(_fixture, world.ID);
        var persona = PersonaFactory.Create(_fixture);
        world = _fixture.CreateContext().Set<World>().Include(w => w.Characters).First(w => w.ID == world.ID);
            return (world, character, persona);
    }
    
    private void CreateCharacters(int count = 1, Guid? world_ID = null){
        var context = _fixture.CreateContext();
        
        for(int i = 0; i < count; i++){
            CharacterFactory.Create(context, world_ID);
        }
    }

    /// <summary>Helper to test that the system message (which is XML formated) indeed does conain the data expected </summary>
    private void assertProperties<T>(List<PropertyInfo> properties, T model, string[] skip, XDocument systemMessage, string tag){
        foreach (var property in properties){
            bool found = systemMessage.Descendants(tag).ToList().Any(element =>
            {
                return element.Value.Contains($"{property.GetValue(model)}");
            });
            Assert.True(found, $"Property '{property.Name}' not found in any {tag} element");
        }
    }

    
    // Tests

    [Fact]
    public void CreateLLMPayload_CorrectReturnType(){
        var (world, character, persona) = CreateData();
        var result = _service.CreateLLMPayload(world, persona);

        Assert.IsType<ServiceResult<LLMPayload>>(result);
    }

    [Fact]
    public void CreateLLMPayload_AlwaysSystemAndEmptyUserOnIntroMessage(){
        var (world, character, persona) = CreateData();
        var payload = _service.CreateLLMPayload(world, persona).Data!;

        Assert.Equal(3, payload.Messages!.Count);
        Assert.Equal(LLMRoles.System, payload.Messages[0].Role);
        Assert.Equal(LLMRoles.User, payload.Messages[1].Role);
        Assert.Equal("", payload.Messages[1].Content);
    }


    [Theory]
    [InlineData(null)]
    [InlineData("Some string")]
    public void CreateLLMPayload_VerifyIntroMessage(string? intro){
        var (world, character, persona) = CreateData();
        world.IntroMessage = intro;
        var payload = _service.CreateLLMPayload(world, persona).Data!;
        
        // If no intro-message then first non-system message will be from the user.
        if(intro == null){
            Assert.Equal(1, payload.Messages!.Count);
        }
        else{
            Assert.Equal("", payload.Messages[1].Content);
            Assert.Equal(LLMRoles.User, payload.Messages[1].Role);
            Assert.Equal(intro, payload.Messages![2].Content);
            Assert.Equal(LLMRoles.Assistant, payload.Messages![2].Role);
            Assert.Equal(3, payload.Messages.Count);
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void CreateLLMPayload_InjectMessages(int messageCount){
        var (world, character, persona) = CreateData();
        bool assistant = true;
        List<Message> messages = Enumerable.Range(0, messageCount).Select(m => {
            var message = new Message
            {
                Content = _faker.Lorem.Lines(),
                Role = assistant ?  LLMRoles.Assistant : LLMRoles.User
            };
            assistant = !assistant;

            return message;
        }).ToList();

        var payloadMessages = _service.CreateLLMPayload(world, persona, messages).Data!.Messages!;

        Assert.Equal(messageCount + 3, payloadMessages.Count);
    }

        

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void CreateLLMPayload_CharactersInformationInSystemMessage(int characterCount){
        World world = WorldFactory.Create(_fixture);
        Persona persona = PersonaFactory.Create(_fixture);
        List<Character> characters = Enumerable.Range(0, characterCount).Select(c => CharacterFactory.Create(_fixture, world.ID)).ToList();
        world.Characters = characters;

        var payload = _service.CreateLLMPayload(world, persona).Data!;
        var systemMessage = XDocument.Parse(payload.Messages![0].Content);
        
        string[] skip = {
            // Doesn't exist in the XML by intent 
            "ID", "Worlds","CreatedAt","UpdatedAt","Description","Name","ID", 
            // NEed to check it manually.
            "ExampleDialogue" 
        };
        
        // Check that the character data does indeed exist as XML Entry
        foreach(var character in characters){

            // Verify the <char> tag contain the name property
            Assert.True(systemMessage.Descendants("char").Any(element => element.Attribute("name")?.Value == character.Name),
                    $"<char> for '{character.Name}' is missing the name attribute");


            assertProperties(typeof(Character).GetProperties().Where(p => !skip.Contains(p.Name)).ToList(), character, skip, XDocument.Parse(payload.Messages[0].Content), "char");

            bool found = systemMessage.Descendants("char").Any(element => character.ExampleDialogue!.All(line => element.Value.Contains(line)));
            Assert.True(found, "ExampleDialogue not found in <char> element");
        }
    }

    [Fact]
    public void CreateLLMPayload_PersonaInformationAddedInSystemMessage(){
        var (world, character, persona) = CreateData();

        var payload = _service.CreateLLMPayload(world, persona).Data!;
        var systemMessage = XDocument.Parse(payload.Messages![0].Content);

        var skip = new[] { "ID", "Name", "CreatedAt", "UpdatedAt", "Description" };
        var properties = typeof(Persona).GetProperties().Where(p => !skip.Contains(p.Name)).ToList();

        assertProperties(typeof(Persona).GetProperties().Where(p => !skip.Contains(p.Name)).ToList(), persona, skip, XDocument.Parse(payload.Messages[0]!.Content), "user");

        // Verify the <user> tag contain the user's name property.
        Assert.True(systemMessage.Descendants("user").Any(element => element.Attribute("name")?.Value == persona.Name),
                $"<char> for '{character.Name}' is missing the name attribute");

    }

    [Fact]
    public void CreateLLMPayload_WorldInformationAddedInSystemMessage(){
        var (world, character, persona) = CreateData();

        var payload = _service.CreateLLMPayload(world, persona).Data!;
        var systemMessage = XDocument.Parse(payload.Messages![0].Content);

        var skip = new[] {"ID", "Lorebook_ID", "Name", "Description", "Characters",  "IntroMessage", "CreatedAt", "UpdatedAt", "NarratorExampleDialogue"};
        var properties = typeof(World).GetProperties().Where(w => !skip.Contains(w.Name)).ToList();

        assertProperties(typeof(World).GetProperties().Where(w => !skip.Contains(w.Name)).ToList(), world, skip, XDocument.Parse(payload.Messages[0]!.Content), "world");
        
        Assert.True(systemMessage.Descendants("world").Any(element => element.Attribute("name")?.Value == world.Name),
                $"<world> for '{world.Name}' is missing the name attribute");
    }
}

