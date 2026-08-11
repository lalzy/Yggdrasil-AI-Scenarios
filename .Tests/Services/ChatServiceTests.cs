// ChatServiceTests.cs

using Yggdrasil.DTO;
using Yggdrasil.Models;
using Yggdrasil.Services;
using Yggdrasil.Tests.Factories;
using Yggdrasil.Extensions;
using Yggdrasil.Util;

namespace Yggdrasil.Test.Services;

public class ChatServiceTests : DatabaseTestBase
{
    private readonly Faker _faker = new();
    private readonly ChatService _service;

    public ChatServiceTests(DatabaseFixture fixture) : base(fixture)
    {
        _service = new ChatService(fixture.CreateContext());
    }

    // Helpers
    private Conversation CreateConversation(){
        var world = WorldFactory.Create(_fixture);
        return ConversationFactory.Create(_fixture, world.ID);
    }
    
    private List<ChatMessage> CreateMessages(int amount, Guid conversation_ID){
        return(Enumerable.Range(0, amount).Select(c => ChatMessageFactory.Create(_fixture, conversation_ID)).ToList());
    }

    private ChatMessageRequest CreateMessageRequest(Guid conversation_ID){
        var request = AutoFaker.Generate<ChatMessageRequest>();
        request.Conversation_ID = conversation_ID;
        return request;
    }
    
    // Tests

    [Theory]
    [InlineData(5)]
    [InlineData(30)]
    [InlineData(150)]
    public void GetAll_GetAllMade(int count){
        var conversation = CreateConversation();
        var messages = CreateMessages(count, conversation.ID);

        var fetched = _service.GetAll(conversation.ID).Data!;
        Assert.Equal(count, fetched.Count);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(30)]
    public void GetAll_GetOnlyRequestedAmount(int count){
        var conversation = CreateConversation();
        var message = CreateMessages(100, conversation.ID);
        var fetched = _service.GetAll(conversation.ID, amount: count).Data!;

        Assert.Equal(count, fetched.Count);
    }

    [Fact]
    public void GetAll_NoErrorOnOverCount(){
        int count = 3;
        var conversation = CreateConversation();
        CreateMessages(3, conversation.ID);

        var fetched = _service.GetAll(conversation.ID, 5).Data!;
        Assert.Equal(count, fetched.Count);
    }

    [Fact]
    public void GetAll_ReturnsCorrectServiceResultType(){
        var conversation = CreateConversation();
        CreateMessages(3, conversation.ID);
        var fetched = _service.GetAll(conversation.ID);

        Assert.IsType<ServiceResult<List<ChatMessageSummary>>>(fetched);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void GetAll_LessThanOneCountThrows(int count){
        var conversation = CreateConversation();
        CreateMessages(3, conversation.ID);
        Assert.Throws<ArgumentException>(()=>_service.GetAll(conversation.ID, amount:count));
    }

    [Fact]
    public void GetAll_EmptyReturnsEmpty(){
        var conversation = CreateConversation();
        var fetched = _service.GetAll(conversation.ID).Data!;
 
        Assert.Empty(fetched);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void GetAll_OnlyOwnedByConversationReturned(int index){
        int count = 5;
        var conversations = Enumerable.Range(0,3).Select(c=>{
            var conv = CreateConversation();
            CreateMessages(count, conv.ID);
            return conv;
        }).ToList();
        var conversationToGet = conversations[index];
        var fetched = _service.GetAll(conversationToGet.ID).Data!;

        Assert.Equal(count, fetched.Count);
    }

    [Fact]
    public void GetAll_CorrectMessageData(){
        var conversation = CreateConversation();
        var messages = CreateMessages(3, conversation.ID);

        var fetched = _service.GetAll(conversation.ID).Data!;

        foreach(var message in messages){
            Assert.Contains(fetched, f => f.ID == message.ID && f.Content == message.Content);
        }
    }

    [Fact]
    public void GetAll_DescendingOrder(){
        var conversation = CreateConversation();
        var date = DateTime.UtcNow;
        var messages = Enumerable.Range(0,5).Select(c => {
            var message = ChatMessageFactory.Create(_fixture, conversation.ID, date);
                date = date.AddHours(1);
                return message;
        }).ToList();

        var fetched = _service.GetAll(conversation.ID).Data!;

        DateTime? last = null;
        foreach(var message in fetched){
            if(!last.HasValue){
                last = message.TimeStamp;
                continue;
            }
            if(last < message.TimeStamp){
                Assert.Fail($"{message.TimeStamp} is newer than {last}");
            }
        }
    }

    [Fact]
    public void GetOne_GetRequested(){
        var conversation = CreateConversation();
        var message = CreateMessages(1, conversation.ID)[0];

        var fetch = _service.GetOne(message.ID).Data!;

        Assert.Equivalent(fetch, message);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void GetOne_GetCorrectFromMany(int index){
        var conversation = CreateConversation();
        var messages = CreateMessages(2, conversation.ID);

        var messageToGet = messages[index];
        var fetch = _service.GetOne(messageToGet.ID).Data!;
        Assert.Equivalent(messageToGet, fetch);
    }

    [Fact]
    public void GetOne_CorrectReturnType(){
        var conversation = CreateConversation();
        var message = ChatMessageFactory.Create(_fixture, conversation.ID);
        var fetch = _service.GetOne(message.ID);

        Assert.IsType <ServiceResult<ChatMessage>>(fetch);
    }

    [Fact]
    public void GetOne_InvalidGuidThrows(){
        Assert.Throws<KeyNotFoundException>(() => _service.GetOne(_faker.Random.Guid()));
    }

    [Fact]
    public void Creates_Success(){
        var conversation = CreateConversation();
        var request = CreateMessageRequest(conversation.ID);
        var message = request.ConvertModelToDTO<ChatMessage>();
        
        var fetch = _service.Create(request).Data!;

        var ID = fetch.ID;
        fetch.ID = Guid.Empty;

        Assert.Equivalent(message, fetch);

        fetch.ID = ID;

        var dbFetch = _fixture.CreateContext().Set<ChatMessage>().FirstOrDefault(c => c.ID == ID);
        Assert.Equivalent(fetch, dbFetch);
    }

    [Fact]
    public void Creates_CorrectReturnType(){
        var fetch = _service.Create(AutoFaker.Generate<ChatMessageRequest>());
        Assert.IsType<ServiceResult<ChatMessage>>(fetch);
    }
}
