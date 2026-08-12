// ConversationServiceTests.cs

using Yggdrasil.Services;
using Yggdrasil.Models;
using Yggdrasil.Tests.Factories;
using Yggdrasil.DTO;
using Yggdrasil.Extensions;
using Yggdrasil.Util;

namespace Yggdrasil.Tests.Services;

public class ConversationServiceTests : DatabaseTestBase
{
    private readonly Faker _faker = new();
    private readonly ConversationService _service;

    public ConversationServiceTests(DatabaseFixture fixture) : base(fixture)
    {
        _service = new ConversationService(fixture.CreateContext());
    }

    private List<Conversation> CreateConversations(int amount, Guid world_ID){
        return Enumerable.Range(0, amount).Select(t => ConversationFactory.Create(_fixture, world_ID)).ToList();
    }

    [Theory]
    [InlineData(5)]
    [InlineData(30)]
    [InlineData(150)]
    public void GetAll_GetAllMade(int count){
        var world = WorldFactory.Create(_fixture);
        CreateConversations(count, world.ID);

        var fetched = _service.GetAllFromWorld(world.ID).Data!;
        Assert.Equal(count, fetched.Count);
    }
    [Theory]
    
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(30)]
    public void GetAll_GetOnlyRequestedAmount(int count){
        var world = WorldFactory.Create(_fixture);
        CreateConversations(150, world.ID);

        var fetched = _service.GetAllFromWorld(world.ID, count).Data!;
        
        Assert.Equal(count, fetched.Count);
    }

    [Fact]
    public void GetAll_NoErrorOnOverCount(){
        int count = 3;
        var world = WorldFactory.Create(_fixture);
        CreateConversations(count, world.ID);
        var fetch = _service.GetAllFromWorld(world.ID, count+5).Data!;
        Assert.Equal(count, fetch.Count);
    }

    [Fact]
    public void GetAll_ReturnsCorrectServiceResultType(){
        var world = WorldFactory.Create(_fixture);
        CreateConversations(3, world.ID);

        var fetch = _service.GetAllFromWorld(world.ID);

        Assert.IsType<ServiceResult<List<ConversationSummary>>>(fetch);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void GetAll_LessThanOneCountThrows (int count){
        var world = WorldFactory.Create(_fixture);
        Assert.Throws<ArgumentException>(()=>_service.GetAllFromWorld(world.ID,count));
    }

    [Fact]
    public void GetAll_EmptyReturnsEmpty(){
        var world = WorldFactory.Create(_fixture);
        var fetch = _service.GetAllFromWorld(world.ID).Data!;

        Assert.Empty(fetch);
    }

    [Fact]
    public void GetAll_WorldNotFoundThrows(){
        ConversationFactory.Create(_fixture);
        Assert.Throws<KeyNotFoundException>(() => _service.GetAllFromWorld(_faker.Random.Guid()));
    }

    [Fact]
    public void GetOne_GetRequested(){
        var world = WorldFactory.Create(_fixture);
        var conversation = ConversationFactory.Create(_fixture, world.ID);

        var fetch = _service.GetOne(conversation.ID).Data!;

        Assert.Equivalent(conversation, fetch);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void GetOne_GetCorrectFromMany(int index){
        var world = WorldFactory.Create(_fixture);
        var conversations = CreateConversations(2, world.ID);
        var conversationToGet = conversations[index];

        var fetch = _service.GetOne(conversationToGet.ID).Data!;

        Assert.Equivalent(conversationToGet, fetch);

        foreach(var other in conversations.Where(c => c != conversationToGet)){
            Assert.Throws<EquivalentException>(() => Assert.Equivalent(other ,fetch));
        }
    }

    [Fact]
    public void GetOne_CorrectReturnType(){
        var world = WorldFactory.Create(_fixture);
        var conversation = ConversationFactory.Create(_fixture, world.ID);
        var fetch = _service.GetOne(conversation.ID);

        Assert.IsType<ServiceResult<Conversation>>(fetch);
    }

    [Fact]
    public void GetOne_InvalidGuidThrows(){
        Assert.Throws<KeyNotFoundException>(()=>_service.GetOne(_faker.Random.Guid()));
    }

    [Fact]
    public void Create_Success(){
        var world = WorldFactory.Create(_fixture);
        var request = AutoFaker.Generate<ConversationRequest>();
        request.World_ID = world.ID;
        var convertedRequest = request.ConvertModelToDTO<Conversation>();
        var conversation = _service.Create(request).Data!;
        var ID = conversation.ID;
        conversation.ID = Guid.Empty;

        Assert.Equivalent(convertedRequest, conversation, strict:false);

        conversation.ID = ID;
        var dbFetch = _fixture.CreateContext().Set<Conversation>().FirstOrDefault(c => c.ID == ID);
        Assert.Equivalent(conversation, dbFetch);
    }

    [Fact]
    public void Create_CorrectReturnType(){
        var world = WorldFactory.Create(_fixture);
        var request = AutoFaker.Generate<ConversationRequest>();
        request.World_ID = world.ID;
        var fetch = _service.Create(request);

        Assert.IsType<ServiceResult<Conversation>>(fetch);
    }

    [Fact]
    public void Create_WorldNotFoundThrows(){
        var request = AutoFaker.Generate<ConversationRequest>();
        Assert.Throws<KeyNotFoundException>(()=>_service.Create(request));
    }
}
