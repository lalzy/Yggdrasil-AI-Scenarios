// LLMConnectionFactory

using System.Text.Json;
using Bogus;
using AutoBogus;
using Yggdrasil.Models.Entities;
using Yggdrasil.Data;
using Yggdrasil.Tests.Factories.BaseRules;

namespace Yggdrasil.Tests.Factories;

public static class LLMConnectionFactory{
    private static Faker _faker = new();

    private static string GenerateResponseJson(string modelName){
        var promptTokens = _faker.Random.Int(10, 500);
        var completionTokens = _faker.Random.Int(10, 1000);
        
        var response = new {
            id = $"chatcmpl-{_faker.Random.AlphaNumeric(24)}",
            @object = "chat.completion",
            created = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            model = modelName,
            choices = new[]{
                new {
                    index = 0,
                    message = new{
                        role = "assistant",
                        content = _faker.Lorem.Paragraph(_faker.Random.Int(1,3))
                    },
                    finish_reason = _faker.PickRandom("stop", "length")
                }
            },
            usage = new{
                prompt_tokens = promptTokens,
                completion_tokens = completionTokens,
                total_tokens = promptTokens + completionTokens
            }
        };

        return JsonSerializer.Serialize(response);
    }

    private static string GenerateRequestJson(string modelName){
        var request = new{
            model = modelName,
            messages = new[]{
                new{role="system", content=_faker.Lorem.Sentence()},
                new{role="user", content=_faker.Lorem.Sentence()}
            },
            temperature = Math.Round(_faker.Random.Double(0, 1), 2),
            max_tokens = _faker.Random.Int(64, 4096),
            stream = false
        };

        return JsonSerializer.Serialize(request);
    }
    
    public static LLMConnection Create(AppDbContext db){
        var model = _faker.Lorem.Word();
        var entity = new AutoFaker<LLMConnection>()
            .IDRule()
            .CreateAndUpdateAtRule()
            .RuleFor(x => x.RequestFormat, _ => GenerateRequestJson(model))
            .RuleFor(x => x.ResponseFormat, _ => GenerateResponseJson(model))
            .Generate();
        db.Set<LLMConnection>().Add(entity);
        db.SaveChanges();
        return entity;
    }
}
