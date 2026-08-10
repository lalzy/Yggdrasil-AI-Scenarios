// ConversationServiceTests.cs

using Yggdrasil.Services;

namespace Yggdrasil.Tests.Services;

public class ConversationServiceTests : DatabaseTestBase
{
    private readonly Faker _faker = new();
    private readonly ConversationService _service;

    public ConversationServiceTests(DatabaseFixture fixture) : base(fixture)
    {
        _service = new ConversationService(fixture.CreateContext());
    }
}
