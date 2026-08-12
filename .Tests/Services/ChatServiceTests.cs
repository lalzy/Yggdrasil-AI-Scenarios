// ChatServiceTests.cs

using Yggdrasil.DTO;
using Yggdrasil.Models;
using Yggdrasil.Services;
using Yggdrasil.Tests.Factories;
using Yggdrasil.Extensions;
using Yggdrasil.Util;
using System.Reflection;

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

    private ChatMessage? GetFromDB(Guid message_ID){
        return _fixture.CreateContext().Set<ChatMessage>().FirstOrDefault(c => c.ID == message_ID);
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

    [Fact]
    public void GetAll_ConversationNotFoundThrows(){
        Assert.Throws<KeyNotFoundException>(() => _service.GetAll(_faker.Random.Guid()));
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

        var dbFetch = GetFromDB(fetch.ID);
        Assert.Equivalent(fetch, dbFetch);
    }

    [Fact]
    public void Creates_CorrectReturnType(){
        var conversation = CreateConversation();
        var request = AutoFaker.Generate<ChatMessageRequest>();
        request.Conversation_ID = conversation.ID;
        var fetch = _service.Create(request);
        Assert.IsType<ServiceResult<ChatMessage>>(fetch);
    }

    [Fact]
    public void Creates_InvalidConversationID(){
        Assert.Throws<KeyNotFoundException>(() => _service.Create(CreateMessageRequest(_faker.Random.Guid())));
        
    }

    [Fact]
    public void Edit_SuccessChangeContent(){
        var conversation = CreateConversation();
        var message = CreateMessages(1, conversation.ID)[0];
        string? newContent;
        // ensure unique new
        do
        {
            newContent = _faker.Lorem.Lines();
        } while (newContent == message.Content);

        var request = new ChatMessageUpdateRequest {Content = newContent };

        // Assert fetch is correct update
        var fetch = _service.Update(message.ID, request).Data!;

        Assert.Equal(newContent, fetch.Content);

        // DB compare
        var dbFetch = GetFromDB(message.ID);
        Assert.Equivalent(fetch, dbFetch);
    }

    [Fact]
    public void Edit_InvalidIDThrows(){
        Assert.Throws<NullReferenceException>(() => _service.Update(_faker.Random.Guid(), AutoFaker.Generate<ChatMessageUpdateRequest>()));
    }

    [Fact]
    public void Edit_ReturnCorrectType(){
        var conversation = CreateConversation();
        var message = ChatMessageFactory.Create(_fixture, conversation.ID);
        var fetch = _service.Update(message.ID, AutoFaker.Generate<ChatMessageUpdateRequest>());
        Assert.IsType<ServiceResult<ChatMessage>>(fetch);
    }

    [Fact]
    public void Edit_OtherFieldsRemainSame(){
        var conversation = CreateConversation();
        var message = ChatMessageFactory.Create(_fixture, conversation.ID);
        string? newContent = null;
        do{
            newContent = _faker.Lorem.Lines();
        } while (newContent == message.Content);
        var request = new ChatMessageUpdateRequest{
            Content = newContent
        };

        var fetch = _service.Update(message.ID, request).Data!;

        foreach(var property in message.GetType().GetProperties()){
            var name = property.Name.ToString().ToLower();
            if(name == "content") continue;
            Assert.Equal(property.GetValue(message), property.GetValue(fetch));

        }
    }

    [Fact]
    public void Delete_Success(){
        var conversation = CreateConversation();
        var message = ChatMessageFactory.Create(_fixture, conversation.ID);

        // Verify exist
        var dbFetch =  GetFromDB(message.ID);
        Assert.NotNull(dbFetch);
        
        _service.Delete(message.ID);

        // Verify deleted
        dbFetch = GetFromDB(message.ID);
        Assert.Null(dbFetch);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void Delete_OnlyRequestedDeleted(int index){
        var conversation = CreateConversation();
        var messages = CreateMessages(3, conversation.ID);
        var messageToDelete = messages[index];
        _service.Delete(messageToDelete.ID);

        foreach(var message in messages){
            var dbFetch = GetFromDB(message.ID);
            if(message == messageToDelete)
                Assert.Null(dbFetch);
            else
                Assert.Equivalent(message, dbFetch);
        }
    }

    [Fact]
    public void Delete_CorrectReturnType(){
        var conversation = CreateConversation();
        var message = ChatMessageFactory.Create(_fixture, conversation.ID);
        var fetch = _service.Delete(message.ID);

        Assert.IsType<ServiceResult<Empty>>(fetch);
    }

    [Fact]
    public void Delete_InvalidGuidThrows(){
        var message = ChatMessageFactory.Create(_fixture);

        Assert.Throws<KeyNotFoundException>(() => _service.Delete(_faker.Random.Guid()));
    }
}
