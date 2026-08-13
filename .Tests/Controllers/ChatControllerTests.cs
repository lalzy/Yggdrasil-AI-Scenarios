// ChatControllerTests.cs

using System.Reflection;
using Yggdrasil.Models;
using Yggdrasil.DTO;
using Yggdrasil.Tests.Util;

namespace Yggdrasil.Tests.Controllers;

public class ChatControllerTests : IClassFixture<WebApplicationFactory<Program>>{
    private readonly Faker _faker = new();
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public ChatControllerTests(WebApplicationFactory<Program> factory){
        _factory = ControllerUtil.Setup(factory);
        _client = _factory.CreateClient();
    }

    // Helpers
    
    private (World, Conversation) CreateConversation(){
        var world = ControllerUtil.CreateWorld(_factory);
        var conversation = ControllerUtil.CreateConversation(_factory, world.ID);
        return (world, conversation);
    }

    private (World, Conversation, ChatMessage) CreateMessageChain(){
        var (world, conversation) = CreateConversation();
        return (world, conversation, ControllerUtil.CreateChatMessage(_factory, conversation.ID));
    }

    private StringContent CreateEditContent(string newContent){
        var body = new {
            Content = newContent
        };

        return  new StringContent(JsonSerializer.Serialize(body), System.Text.Encoding.UTF8, "application/json");
    }

    // Tests
    
    [Fact]
    public async Task GetAll(){
        var conversation = ControllerUtil.CreateConversation(_factory);
        var response = await _client.GetAsync($"/api/chat/all/{conversation.ID}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(50)]
    public async Task GetAll_OkOnCount(int count){
        var conversation = ControllerUtil.CreateConversation(_factory);
        var response = await _client.GetAsync($"/api/chat/all/{conversation.ID}?count={count}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Theory]
    [InlineData("abc")]
    [InlineData("-1")]
    public async Task GetAll_BadCount_ReturnsBadRequest(string count){
        var conversation = ControllerUtil.CreateConversation(_factory);
        var response = await _client.GetAsync($"/api/chat/all/{conversation.ID}?count={count}");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_NotFoundConversation(){
        var response = await _client.GetAsync($"/api/chat/all/{_faker.Random.Guid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetOne_Ok(){
        var (world, conversation) = CreateConversation();
        var response = await _client.GetAsync($"/api/chat/{conversation.ID}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetOne_NotFound(){
        var response = await _client.GetAsync($"/api/chat/{_faker.Random.Guid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_Ok(){
        var (world, conversation) = CreateConversation();
        var body = new {
            conversation_ID = conversation.ID,
            Role = RoleType.user,
            Content = _faker.Lorem.Lines()
        };
        var content = new StringContent(JsonSerializer.Serialize(body), System.Text.Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/chat/create/", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("Role")]
    [InlineData("Content")]
    public async Task Create_MissingRequiredReturnBadRequest(string propertyToSkip){
        var (world, conversation) = CreateConversation();
        var body = typeof(ChatMessageRequest).GetProperties()
            .ToDictionary(p => p.Name, p => p.Name == propertyToSkip 
                ? (object)null
                : p.PropertyType switch {
                    Type t when t == typeof(Guid) => (object)conversation.ID,
                    Type t when t == typeof(RoleType) => (object)RoleType.user,
                    Type t when t == typeof(string) => (object)_faker.Lorem.Lines()
            });
        
        var content = new StringContent(JsonSerializer.Serialize(body), System.Text.Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/chat/create", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_NotFoundConversation(){
        var body = new {
            conversation_ID = _faker.Random.Guid(),
            Role = RoleType.user,
            Content = _faker.Lorem.Lines()
        };
        var content = new StringContent(JsonSerializer.Serialize(body), System.Text.Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/chat/create", content);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_Ok(){
        var (world, conversation, message) = CreateMessageChain();
        string? newContent;
        
        do{
            newContent = _faker.Lorem.Lines();
        }while(newContent == message.Content);
        
        var content = CreateEditContent(newContent);
        var response = await _client.PatchAsync($"/api/chat/{message.ID}", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Update_NotFoundMessage(){
        
        var content = CreateEditContent(_faker.Lorem.Lines());
        var response = await _client.PatchAsync($"/api/chat/{_faker.Random.Guid()}", content);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_NoContentOnSuccess(){
        var (world, conversation, message) = CreateMessageChain();
        var response = await _client.DeleteAsync($"/api/chat/{message.ID}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_NotFound(){
        var response = await _client.DeleteAsync($"/api/chat/{_faker.Random.Guid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
