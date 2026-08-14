// LLMControllerTests

using Yggdrasil.DTO;
using Yggdrasil.Tests.Util;

namespace Yggdrasil.Tests.Controllers;

public class LLMControllerTests : IClassFixture<WebApplicationFactory<Program>>{
    private readonly Faker _faker = new();
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public LLMControllerTests(WebApplicationFactory<Program> factory){
        _factory = ControllerUtil.Setup(factory);
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Create_Ok(){
        var world = ControllerUtil.CreateWorld(_factory);
        var persona = ControllerUtil.CreatePersona(_factory);
        var body = new { World = world, Persona = persona};
        
        var content = new StringContent(JsonSerializer.Serialize(body), System.Text.Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/llm/create", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("World")]
    [InlineData("Persona")]
    public async Task Create_BadRequestOnMissingRequired(string propertyToSkip){
        
        var world = ControllerUtil.CreateWorld(_factory);
        var persona = ControllerUtil.CreatePersona(_factory);
        var body = new LLMPayloadRequest { World = world, Persona = persona};
        body.GetType().GetProperty(propertyToSkip)?.SetValue(body, null);     
        
        var content = new StringContent(JsonSerializer.Serialize(body), System.Text.Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/llm/create", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
    }
}
