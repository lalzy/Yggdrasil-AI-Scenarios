// LLMService.cs

using Yggdrasil.Data;
using Yggdrasil.Services.Templates;
using Yggdrasil.Models.Entities;
using Yggdrasil.Models.Entities.Templates;
using Yggdrasil.Models.Entities.Instances;
using Yggdrasil.Models.DTO;
using Yggdrasil.Models.Enums;
using Yggdrasil.Extensions;
using System.Linq.Dynamic.Core;
using Mapster;
using Yggdrasil.Utils;
using System.Text.Json;
using System.Text.Json.Nodes;
using Yggdrasil.Models.ValueObjects;
using Yggdrasil.Models.Interfaces;
using System.Collections.Generic;
using System.Text;

namespace Yggdrasil.Services;

public  class LLMService (AppDbContext db){
    readonly DbSet<LLMConnection> _connections = db.Set<LLMConnection>();
    readonly DbSet<Conversation> _conversations = db.Set<Conversation>();

    /// <summary>Replace placeholders inside a JSON Array, where objects are resolved recursively.</summary>
    /// <remarks> {{Messages}} gets replaced by the <paramref="chatMessages"/>.</remarks>
    private void CreateRequestArray(JsonArray array, LLMSettings settings, List<Chat> chatMessages, string systemMessage){
        foreach(var item in array.ToList()){
            if (item is JsonObject json){
                CreateRequestJson(json, settings, chatMessages, systemMessage);
                continue;
            }

            if(item is not JsonValue jsonValue || !jsonValue.TryGetValue<string>(out var placeholder) || placeholder != "{{Messages}}"){
                continue;
            }

            var index = array.IndexOf(item);
            array.RemoveAt(index);

            foreach(var message in ConvertChatMessagesToAnJsonNodeArray(chatMessages)){
                array.Insert(index++, message);
            }
        }
    }

    /// <summary>Convers the ChatMessages into an JsonArray</summary>
    private JsonNode?[] ConvertChatMessagesToAnJsonNodeArray(List<Chat> chatMessages){
        return chatMessages.Select(c => JsonSerializer.SerializeToNode(new {role = c.Role, content=c.Content})).ToArray();
    }
  
    /// <summary>Create a valid provider JSON request based on user supplied JsonRequest Form</summary>
    private object CreateRequestJson(JsonObject requestJson, LLMSettings settings, List<Chat> chatMessages, string systemMessage){
        foreach(var (key, value) in requestJson.ToList()){
            if(value is JsonObject child){
                CreateRequestJson(child, settings, chatMessages, systemMessage);
                continue;
            }else if(value is JsonArray array){
                CreateRequestArray(array, settings, chatMessages, systemMessage);
            }
            
            if(value is not JsonValue v || !v.TryGetValue<string>(out var placeHolder)){
                continue;
            }

            // Strip opening and closing brackets
            var name = placeHolder[2..^2];
            var property = typeof(LLMSettings).GetProperty(name);

            if(name == "SystemMessage"){
                requestJson[key] = systemMessage;
            }else if(name == "Messages"){
                requestJson[key] = new JsonArray(ConvertChatMessagesToAnJsonNodeArray(chatMessages));
            }else if(property != null){
                requestJson[key] = JsonSerializer.SerializeToNode(property.GetValue(settings));
            }
        }
        return requestJson;
    }
    
    /// <summary>Create an LLMRequestJsonObject to be sent to an LLM Provider (such as Llama.cpp, openrouter, Anthropic).</summary>
    /// <param name="connectionID">Connection to be used</param>
    /// <returns>Provider formated Object(to be passed as a Json)</returns>
    public object CreateRequest(Guid connectionID, Guid conversationID, string systemMessage){
        var connection = _connections.Find(connectionID)!;
        var settings = connection.Settings;
        var conversation = _conversations.Include(c => c.Chats).First(c => c.ID == conversationID);
        
        return CreateRequestJson(JsonNode.Parse(connection.RequestFormat)!.AsObject(), settings, conversation.Chats, systemMessage);
    }

    public ResponseDTO CreateResponse(Guid connectionID){
        var entity = new ResponseDTO{
                FinishReason = "", Message = new MessageDTO(), Model = ""};
        return entity;
    
    }
}
