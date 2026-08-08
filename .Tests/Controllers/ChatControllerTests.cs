// ChatControllerTests.cs

using Yggdrasil.Tests.Factories;
using Yggdrasil.Tests.Util;

using Yggdrasil.Models;

namespace Yggdrasil.Tests.Controllers;

public class ChatControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly Faker _faker = new();
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public ChatControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = ControllerUtil.Setup(factory);
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Send_Success()
    {
        var connection = ControllerUtil.CreateConnection(_factory, Models.SupportedProviders.OpenRouter);
        var payload = LLMPayloadFactory.Create();
        var body = new
        {
            Connection = connection,
            Payload = payload
        };
        var content = new StringContent(JsonSerializer.Serialize(body), System.Text.Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/chat/send", content);
    }

    [Fact]
    public async Task Send_ConcurrentRequests_DoNotCrosscontaminate_ApiKeys()
    {
        var connection1 = ControllerUtil.CreateConnection(_factory, Models.SupportedProviders.OpenRouter);
        var connection2 = ControllerUtil.CreateConnection(_factory, Models.SupportedProviders.OpenRouter);
        connection1.APIKey = "key-one";
        connection2.APIKey = "key-two";

        var payload = LLMPayloadFactory.Create();

        var body1 = new { Connection = connection1, Payload = payload };
        var body2 = new { Connection = connection2, Payload = payload };

        var content1 = new StringContent(JsonSerializer.Serialize(body1), System.Text.Encoding.UTF8, "application/json");
        var content2 = new StringContent(JsonSerializer.Serialize(body2), System.Text.Encoding.UTF8, "application/json");

        var task1 = _client.PostAsync("/api/chat/send", content1);
        var task2 = _client.PostAsync("/api/chat/send", content2);

        var results = await Task.WhenAll(task1, task2);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Send_()
    {
        
    }
}
