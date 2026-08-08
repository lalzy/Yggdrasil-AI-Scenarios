// ChatServiceTests.cs

using Yggdrasil.Tests.Util;
using Yggdrasil.Services;
using Yggdrasil.DTO;
using Yggdrasil.Util;
using Yggdrasil.Models;
using Yggdrasil.Tests.Factories;
using Yggdrasil.Constants;

namespace Yggdrasil.Tests.Services;

public class ChatServiceTests : DatabaseTestBase{
    private readonly ChatService _service;
    private readonly Faker _faker = new();
    private readonly FakeHttpHandler _handler;

    public ChatServiceTests(DatabaseFixture fixture) : base (fixture){
        _handler = new FakeHttpHandler();
        var client = new HttpClient(_handler);
        _service = new ChatService(client);
    }


    private record ResponseData{
        public string Model { get; set; }
        public int CompletionTokens { get; set; }
        public int PromptTokens { get; set; }
        public int TotalTokens { get; set; }
        public string Content { get; set; }
        public decimal Cost { get; set; }
        public string Role { get; set; }
        public string FinishReason { get; set; }
        public string? Refusal { get; set; }
        public string? Reasoning { get; set; }
    }

    private ResponseData InitData() => new ResponseData
    {
        Model = _faker.Lorem.Word(),
        CompletionTokens = _faker.Random.Int(min: 1),
        PromptTokens = _faker.Random.Int(min: 1),
        TotalTokens = _faker.Random.Int(min: 2),
        Content = _faker.Lorem.Lines(),
        Cost = 0.002M,
        Role = LLMRoles.Assistant,
        FinishReason = "stop",
        Refusal = _faker.Lorem.Lines(),
        Reasoning = _faker.Lorem.Lines()
    };

    private LLMConnectionRequest CreateRequest(LLMConnection connection){
        return connection.ConvertModelToDTO<LLMConnectionRequest>();
    }
    
    // Create a mock response of OpenRouter
    private ResponseData OpenRouterMock(ResponseData? data=null){
        data ??= InitData();
        
        _handler.Response = JsonSerializer.Serialize(new {
            id = $"gen-{_faker.Random.AlphaNumeric(14)}",
            choices = new[] {
                new {
                    finish_reason = data.FinishReason,
                    native_finish_reason = data.FinishReason,
                    message = new {
                        role = data.Role,
                        content = data.Content,
                        refusal = data.Refusal,
                        reasoning = data.Reasoning,
                    }
                }
            },
            usage = new {
                prompt_tokens = data.PromptTokens,
                completion_tokens = data.CompletionTokens,
                total_tokens = data.TotalTokens,
                prompt_tokens_details = new {
                    cached_tokens = 0
                },
                completion_tokens_details = new {
                    reasoning_tokens = 0
                },
                cost = data.Cost
            },
            model = data.Model,
            @object = "chat.completion"
        }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });


        return data;
    }

    [Fact]
    public async Task Send_CorrectReturn(){
        var response = OpenRouterMock();
        var connection = CreateRequest(LLMConnectionFactory.Create(_fixture, SupportedProviders.OpenRouter));
        var payload = LLMPayloadFactory.Create();

        var result = (await _service.Send(connection, payload));

        Assert.IsType<ServiceResult<LLMResponse>>(result);
    }

    [Fact]
    public async Task Send_CorrectData(){

        var response = OpenRouterMock();
        

        var connection = CreateRequest(LLMConnectionFactory.Create(_fixture, SupportedProviders.OpenRouter));
        var payload = LLMPayloadFactory.Create();

        var result = (await _service.Send(connection, payload)).Data!;
        Assert.Equal(response.Content, result.Response);
        Assert.Equal(response.Model, result.Model);
        Assert.Equal(response.Role, result.Role);
        Assert.Equal(response.FinishReason, result.FinishReason);
        Assert.Equal(response.Refusal, result.Refusal);
        Assert.Equal(response.Reasoning, result.Reasoning);

        // Usage
        Assert.Equal(response.Cost, result.Usage.Cost);
        Assert.Equal(response.CompletionTokens, result.Usage.CompletionTokens);
        Assert.Equal(response.PromptTokens, result.Usage.PromptTokens);
        Assert.Equal(response.TotalTokens, result.Usage.TotalTokens);
    }

    [Fact]
    public async Task Send_NullFields(){
        var data = InitData();
        data.Refusal = null;
        data.Reasoning = null;
        var response = OpenRouterMock(data);

        var connection = CreateRequest(LLMConnectionFactory.Create(_fixture, SupportedProviders.OpenRouter));
        var payload = LLMPayloadFactory.Create();

        var result = (await _service.Send(connection, payload)).Data!;

        Assert.Equal(data.Refusal, result.Refusal);
        Assert.Equal(data.Reasoning, result.Reasoning);
    }

    [Fact]
    public async Task Send_UnsupportedProviderThrows(){
        var data = InitData();
        var response = OpenRouterMock(data);

        var connection = CreateRequest(LLMConnectionFactory.Create(_fixture, (SupportedProviders)99999));
        var payload = LLMPayloadFactory.Create();

        await Assert.ThrowsAsync<NotSupportedException>(()=> _service.Send(connection, payload));
    }

    [Fact]
    public async Task Send_MalformedJsonThrows(){
        var data = "invalid";
        _handler.Response = JsonSerializer.Serialize(new {
            incorrect = "incorrectDataFormat"
        }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        var connection = CreateRequest(LLMConnectionFactory.Create(_fixture, SupportedProviders.OpenRouter));
        var payload = LLMPayloadFactory.Create();

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.Send(connection, payload));
    }

    [Fact]
    public async Task Send_AuthorizationHeader(){
        OpenRouterMock();
        var connection = CreateRequest(LLMConnectionFactory.Create(_fixture, SupportedProviders.OpenRouter));
        var payload = LLMPayloadFactory.Create();

        await _service.Send(connection, payload);

        var auth = _handler.PreviousRequest!.Headers.Authorization!;

        Assert.Equal("Bearer", auth.Scheme);
        Assert.Equal(connection.APIKey, auth.Parameter);
    }
}
