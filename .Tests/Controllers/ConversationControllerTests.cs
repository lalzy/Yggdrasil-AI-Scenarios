// ConversationControllerTests.cs

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
}
