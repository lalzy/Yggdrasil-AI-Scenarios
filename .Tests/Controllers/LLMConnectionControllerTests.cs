// LLMConnectionControllerTests.cs

using Yggdrasil.DTO;
using Yggdrasil.Tests.Util;
using System.Text.RegularExpressions;

namespace Yggdrasil.Tests.Controllers;

public class LLMConnectionControllerTests : IClassFixture<WebApplicationFactory<Program>>{
    private readonly Faker _faker = new();
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public LLMConnectionControllerTests(WebApplicationFactory<Program> factory){
        _factory = ControllerUtil.Setup(factory);
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_Ok(){
        var response = await _client.GetAsync($"/api/connection/all");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("10")]
    [InlineData("150")]
    public async Task GetAll_OkOnCount(string count){
        var response = await _client.GetAsync($"/api/connection/all?count={count}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("-1")]
    public async Task GetAll_BadCount_ReturnsBadRequest(string count){
        var response = await _client.GetAsync($"/api/connection/all?count={count}");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetOne_Ok(){
        var connection = ControllerUtil.CreateConnection(_factory);
        var response = await _client.GetAsync($"/api/connection/{connection.ID}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetOne_NotFound(){
        var response = await _client.GetAsync($"/api/connection/{_faker.Random.Guid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_Ok(){
        var body = AutoFaker.Generate<LLMConnectionRequest>();

        var content = new StringContent(JsonSerializer.Serialize(body), System.Text.Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/connection/create", content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("Name")]
    [InlineData("APIType")]
    [InlineData("URL")]
    [InlineData("Provider")]
    public async Task Create_MissingRequiredReturnBadRequest(string property){
        var body = AutoFaker.Generate<LLMConnectionRequest>();
        var content = new StringContent(Regex.Replace(JsonSerializer.Serialize(body), $"\"{property}\":[^, }}]", $"\"{property}\":null"), System.Text.Encoding.UTF8, "application/json");
        body.GetType().GetProperty(property)?.SetValue(body, null);        
        
        var response = await _client.PostAsync("/api/connection/create", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Delete_NoContentOnSuccess(){
        var connection = ControllerUtil.CreateConnection(_factory);
        var response = await _client.DeleteAsync($"/api/connection/{connection.ID}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_NotFound(){
        var response = await _client.DeleteAsync($"/api/connection/{_faker.Random.Guid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
