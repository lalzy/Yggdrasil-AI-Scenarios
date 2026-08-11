// LLMSendService.cs
using System.Net.Http.Headers;
using Yggdrasil.DTO;
using Yggdrasil.Util;
using Yggdrasil.Extensions;
using Yggdrasil.Models;
using System.Text;
using System.Text.Json;
using Yggdrasil.Constants;

namespace Yggdrasil.Services;

public class LLMSendService
{

    private readonly HttpClient _client;
    public LLMSendService(HttpClient client){
        _client = client;
    }

    // Quick and "static" for now, will 'probably' make a config-based approach instead later to make it easy
    // to add new providers.
    ///<summary>Parse Json data from OpenRouter endpoint to an LLMResponse object</summary>
    ///<returns>The LLMResponse object</returns>
    private LLMResponse ParseOpenRouter(JsonElement data){
        var model = data.GetProperty("model").GetString();
        var usage = data.GetProperty("usage");
        var choice = data.GetProperty("choices")[0];
        var message = choice.GetProperty("message");


        var Parsed = new LLMResponse{
            Model = model,
            Usage = new LLMUsage {
                PromptTokens=usage.GetProperty("prompt_tokens").GetInt32(),
                CompletionTokens=usage.GetProperty("completion_tokens").GetInt32(),
                TotalTokens=usage.GetProperty("total_tokens").GetInt32(),
                Cost=usage.GetProperty("cost").GetDecimal()
            },
            Response = message.GetProperty("content").GetString() ?? "",
            Role = message.GetProperty("role").GetString() ?? "",
            Refusal = message.GetProperty("refusal").GetString(),
            Reasoning = message.GetProperty("reasoning").GetString(),
            FinishReason = choice.GetProperty("finish_reason").GetString(),
        };

        
        return Parsed;
    }

    /// <summary>Sends the json to the LLM endpoint</summary>
    /// <returns>The Http Response object</returns>
    private async Task<HttpResponseMessage> SendToLLMService(string URL, string? APIKey, string json){
        var request = new HttpRequestMessage(HttpMethod.Post, URL);
        if(APIKey != null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", APIKey);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        return (await _client.SendAsync(request));
    }

    /// <summary>Create the request body from the payload and connection</summary>
    /// <returns>The body to be made into a Json string</returns>
    private Dictionary<string, JsonElement> CreateBody(LLMConnectionRequest connection, LLMPayload payload){
        var body = ObjectMerger.Merge([payload, connection]);
        body["reasoning"] = JsonSerializer.SerializeToElement(new { enabled = connection.Reasoning });
        body.Remove("name");
        
        return body;
    }
    
    ///<summary></summary>
    ///<param name="connection">The LLM Connection object, which contains APIKey, URL, Model, etc.</param>
    ///<param name="payload">The messages/chatlogs to send to the LLM</param>
    ///<returns>The LLM response</returns>
    ///<exception cref="NotSupportedException">Provider not supported</exception>
    public async Task<ServiceResult<LLMResponse>> Send(LLMConnectionRequest connection, LLMPayload payload)
    {
        var body = CreateBody(connection, payload);
        var json = JsonSerializer.Serialize(body, new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase});

        var response = await SendToLLMService(connection.URL, connection.APIKey, json);
        var result = await response.Content.ReadAsStringAsync();

        // parse and return an LLM Response
        switch(connection.Provider){
            case SupportedProviders.OpenRouter:
                return ServiceResult<LLMResponse>.Ok(ParseOpenRouter(JsonSerializer.Deserialize<JsonElement>(result)));
            default:
                throw new NotSupportedException(ErrorMessages.INVALID_PROVIDER);
        }
    }
}
