// LLMServiceTests.cs

using FluentAssertions;
using AutoBogus;
using Bogus;
using Yggdrasil.Tests.TestUtil;
using Yggdrasil.Services;
using Yggdrasil.Models.Entities;
using Yggdrasil.Models.DTO;
using Microsoft.EntityFrameworkCore; 
using Yggdrasil.Tests.Factories;
using Yggdrasil.Models.Enums.SortOrders;
using System.Linq.Dynamic.Core;
using System.Collections;
using System.Text.Json.Nodes;

namespace Yggdrasil.Tests.Services;

public class LLMServiceTests : DatabaseSetup{
    private readonly LLMService _service;
    private readonly Faker _faker = new();

    public LLMServiceTests(){
        _service = new LLMService(Db());
    }
    
    // =======================================
    //             CREATEREQUEST            
    // =======================================

    public static TheoryData<string, string> RequestFormats => new(){
        {
            "LlamaCpp",
            
            """
            {
                "messages": [
                    {"role":"system", "content":"{{SystemMessage}}"},
                    "{{Messages}}"
                ],
                "max_tokens": "{{MaxTokens}}",
                "temperature": "{{Temperature}}",
                "top_p": "{{TopP}}",
                "stream": "{{Streaming}}",
                "chat_template_kwargs": {
                    "enable_thinking": "{{Reasoning}}",
                    "reasoning_effort": "{{ReasoningEffort}}"
                }
            }
            """
        },
        {
            "Anthropic",
            """
            {
                "system": "{{SystemMessage}}",
                "messages": "{{Messages}}",
                "model": "{{Model}}",
                "max_tokens": "{{MaxTokens}}",
                "temperature": "{{Temperature}}",
                "top_p": "{{TopP}}",
                "stream": "{{Streaming}}",
                "output_config":{
                    "effort":"{{ReasoningEffort}}"
                }
            }
            """
        }
    };

    [Theory]
    [MemberData(nameof(RequestFormats))]
    public void CreateRequest_PopulateSettings(string provider, string requestFormat){
        var db = Db();
        var world = WorldFactory.CreateInstance(db);
        var conversation = ConversationFactory.Create(db, world);
        var systemMessage = _faker.Lorem.Word();
        
        var connection = LLMConnectionFactory.Create(Db(), requestFormat: requestFormat);
        var fetch = _service.CreateRequest(connection.ID, conversation.ID, systemMessage);
        var expected = JsonNode.Parse(requestFormat)!.AsObject();
        var actual = JsonNode.Parse(fetch.ToString()!)!.AsObject();
        
        Assert.Equal(expected.Select(p => p.Key), actual.Select(p => p.Key));
        Assert.DoesNotContain("{{", actual.ToJsonString());
    }

    [Theory]
    [MemberData(nameof(RequestFormats))]
    public void CreateRequest_MessagesAreAddedCorrectly(string provider, string requestFormat){
        var db = Db();
        var world = WorldFactory.CreateInstance(db);
        var conversation = ConversationFactory.Create(db, world);
        var chats = Enumerable.Range(0,10).Select(_ => ChatFactory.Create(db, conversation)).ToList();
        var systemMessage = _faker.Lorem.Word();

        var connection = LLMConnectionFactory.Create(Db(), requestFormat: requestFormat);
        var fetch = _service.CreateRequest(connection.ID, conversation.ID, systemMessage);
        var actual = JsonNode.Parse(fetch.ToString()!)!.AsObject();

        var messages = actual["messages"]!.AsArray()
            .Select(m => m!["content"]!.GetValue<string>())
            .ToList();

        if(actual.ContainsKey("system")){
            Assert.Equal(systemMessage, actual["system"]!.GetValue<string>());
        }else{
            Assert.Equal(systemMessage, messages[0]);
            messages.RemoveAt(0);
        }

        Assert.Equal(chats.Select(c => c.Content), messages);
    }
}
