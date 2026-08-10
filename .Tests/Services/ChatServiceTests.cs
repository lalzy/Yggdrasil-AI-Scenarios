// ChatServiceTests.cs

using Yggdrasil.Services;

namespace Yggdrasil.Test.Services;

public class ChatServiceTests : DatabaseTestBase
{
    private readonly Faker _faker = new();
    private readonly ChatService _service;

    public ChatServiceTests(DatabaseFixture fixture) : base(fixture)
    {
        _service = new ChatService(fixture.CreateContext());
    }
}
