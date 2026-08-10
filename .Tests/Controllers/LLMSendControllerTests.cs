// ChatControllerTests.cs

using System.Text.RegularExpressions;
using Yggdrasil.Tests.Factories;
using Yggdrasil.Tests.Util;
using Yggdrasil.Util;
using Yggdrasil.DTO;
using Yggdrasil.Services;

namespace Yggdrasil.Tests.Controllers;

public class LLMSendControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    
    private readonly Faker _faker = new();
    private readonly FakeHttpHandler _handler;
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public LLMSendControllerTests(WebApplicationFactory<Program> factory)
    {
        (_factory, _handler) = ControllerUtil.SetupWithHandler(factory, services =>
    {
        services.AddSingleton<LLMSendService>(_ => new LLMSendService(new HttpClient(_handler)));
        return services;
    });
        _client = _factory.CreateClient();
    }

    private LLMConnectionRequest createRequest(){
        return ControllerUtil.CreateConnection(_factory, Models.SupportedProviders.OpenRouter).ConvertModelToDTO<LLMConnectionRequest>();
    }

    [Fact]
    public async Task Send_Success()
    {
        var connection = createRequest();
        var payload = LLMPayloadFactory.Create();
        var body = new
        {
            Connection = connection,
            Payload = payload
        };
        var content = new StringContent(JsonSerializer.Serialize(body), System.Text.Encoding.UTF8, "application/json");


        _handler.Response = MockResponses.OpenRouter();

        var response = await _client.PostAsync("/api/chat/send", content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("APIType")]
    [InlineData("URL")]
    [InlineData("Provider")]
    [InlineData("Messages")]
    public async Task Send_RequiredMembers(string property)
    {
        var connection = createRequest();
        var payload = LLMPayloadFactory.Create();

        var body = new
        {
            Connection = connection,
            Payload = payload
        };
        var content = new StringContent(Regex.Replace(JsonSerializer.Serialize(body), $"\"{property}\":[^,}}]+", $"\"{property}\":null"),
            System.Text.Encoding.UTF8,
            "application/json"
        );
        var response = await _client.PostAsync("/api/chat/send", content);


        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
}
