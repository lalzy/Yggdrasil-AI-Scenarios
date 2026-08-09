// MockResponses.cs

namespace Yggdrasil.Tests.Util;

class MockResponses{
    private static Faker _faker = new();

    public static string OpenRouter(){
        return JsonSerializer.Serialize(new {
            id = $"gen-{_faker.Random.AlphaNumeric(14)}",
            choices = new[] {
                new {
                    finish_reason = "stop",
                    native_finish_reason = "stop",
                    message = new {
                        role = "assistant",
                        content = _faker.Lorem.Sentence(),
                        refusal = (string?)null,
                        reasoning = (string?)null,
                    }
                }
            },
            usage = new {
                prompt_tokens = _faker.Random.Int(1, 100),
                completion_tokens = _faker.Random.Int(1, 100),
                total_tokens = _faker.Random.Int(1, 200),
                prompt_tokens_details = new { cached_tokens = 0 },
                completion_tokens_details = new { reasoning_tokens = 0 },
                cost = _faker.Random.Double(0, 1)
            },
            model = _faker.Lorem.Word(),
            @object = "chat.completion"
        }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    }
}
