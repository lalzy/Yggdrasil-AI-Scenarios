// ChatServiceTests.cs

using Yggdrasil.DTO;
using Yggdrasil.Models;
using Yggdrasil.Services;
using Yggdrasil.Tests.Factories;
using Yggdrasil.Extensions;

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
}
