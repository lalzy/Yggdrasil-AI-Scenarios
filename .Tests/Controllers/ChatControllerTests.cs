// ChatControllerTests.cs

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
}
