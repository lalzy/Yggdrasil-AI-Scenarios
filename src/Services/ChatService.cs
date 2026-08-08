// ChatService.cs
using System.Net.Http.Headers;
using System.Xml.Linq;
using Yggdrasil.Data;
using Yggdrasil.DTO;
using Yggdrasil.Util;
using Yggdrasil.Models;
using System.Text;
using System.Text.Json;
using Yggdrasil.Constants;

namespace Yggdrasil.Services;

public class ChatService
{

    private readonly HttpClient _client;
    public ChatService(HttpClient client){
        _client = client;
    }

    // Quick and "static" for now, will 'probably' make a config-based approach instead later to make it easy
    // to add new providers.
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
    
    ///<summary></summary>
    ///<param name="connection">The LLM Connection object, which contains APIKey, URL, Model, etc.</param>
    ///<param name="payload">The messages/chatlogs to send to the LLM</param>
    ///<returns>The LLM response</returns>
    public async Task<ServiceResult<LLMResponse>> Send(LLMConnectionRequest connection, LLMPayload payload)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", connection.APIKey);

        var body = ObjectMerger.Merge([payload, connection]);
        body["reasoning"] = JsonSerializer.SerializeToElement(new { enabled = connection.Reasoning });
        body.Remove("name");
        var json = JsonSerializer.Serialize(body, new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _client.PostAsync(connection.URL, content);
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
