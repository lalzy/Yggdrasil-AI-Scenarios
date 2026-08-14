// ConversationControllerTests.cs

using Yggdrasil.DTO;
using Yggdrasil.Tests.Util;

namespace Yggdrasil.Tests.Controllers;

public class ConversationControllerTests : IClassFixture<WebApplicationFactory<Program>>{
    private readonly Faker _faker = new();
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public ConversationControllerTests(WebApplicationFactory<Program> factory){
        _factory = ControllerUtil.Setup(factory);
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetAllFromWorld_Ok(){
        var world = ControllerUtil.CreateWorld(_factory);
        var response = await _client.GetAsync($"/api/conversation/all/{world.ID}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("10")]
    [InlineData("150")]
    public async Task GetAllFromWorld_OkOnCount(string count){
        var world = ControllerUtil.CreateWorld(_factory);
        var response = await _client.GetAsync($"/api/conversation/all/{world.ID}?count={count}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("-1")]
    public async Task GetAllFromWorld_BadCount_ReturnsBadRequest(string count){
        var world = ControllerUtil.CreateWorld(_factory);
        var response = await _client.GetAsync($"/api/conversation/all/{world.ID}?count={count}");
    }

    [Fact]
    public async Task GetAllFromWorld_NotFound(){
        var response = await _client.GetAsync($"/api/conversation/all/{_faker.Random.Guid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetOne_Ok(){
        var (world, conversation) = CreateHelpers.CreateConversation(_factory);
        var response = await _client.GetAsync($"/api/conversation/{conversation.ID}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetOne_NotFound(){
        var response = await _client.GetAsync($"/api/conversation/{_faker.Random.Guid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_Ok(){
        var world = ControllerUtil.CreateWorld(_factory);
        var body = AutoFaker.Generate<ConversationRequest>();
        body.World_ID = world.ID;

        var content = new StringContent(JsonSerializer.Serialize(body), System.Text.Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/conversation/create", content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("Title")]
    public async Task Create_MissingRequiredReturnBadRequest(string property){
        
        var world = ControllerUtil.CreateWorld(_factory);
        var body = AutoFaker.Generate<ConversationRequest>();
        body.World_ID = world.ID;

        body.GetType().GetProperty(property)?.SetValue(body, null);        
        
        var content = new StringContent(JsonSerializer.Serialize(body), System.Text.Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/conversation/create", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_MissingWorldThrowNotFound(){
        var body = AutoFaker.Generate<ConversationRequest>();
        body.World_ID = Guid.Empty;
        
        var content = new StringContent(JsonSerializer.Serialize(body), System.Text.Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/conversation/create", content);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_NoContentOnSuccess(){
        var (world, conversation) = CreateHelpers.CreateConversation(_factory);
        var response = await _client.DeleteAsync($"/api/conversation/{conversation.ID}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_NotFound(){
        var response = await _client.DeleteAsync($"/api/conversation/{_faker.Random.Guid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
