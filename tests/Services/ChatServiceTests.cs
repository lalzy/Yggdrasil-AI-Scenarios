// ChatServiceTests.cs

using FluentAssertions;
using AutoBogus;
using Bogus;
using System.Text.Json;
using Yggdrasil.Tests.TestUtil;
using Yggdrasil.Services;
using Yggdrasil.Models.Entities;
using Yggdrasil.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Yggdrasil.Tests.Factories;
using Yggdrasil.Models.Enums.SortOrders;
using Yggdrasil.Data;

namespace Yggdrasil.Tests.Services;

public class ChatServiceTests : DatabaseSetup{
    private readonly ChatService _service;
    private readonly Faker _faker = new();
    private DbSet<Conversation> Conversation => Db().Set<Conversation>();
    private DbSet<Chat> Chat => Db().Set<Chat>();

    public ChatServiceTests(){
        _service = new ChatService(Db());
    }

    // =======================================
    //                 CREATE            
    // =======================================

    [Fact]
    public void Create_CreatesChat(){
        var request = AutoFaker.Generate<ChatDTO.Request>();
        var db = Db();
        var world = WorldFactory.CreateInstance(db);
        var conversation = ConversationFactory.Create(db, world);
        var fetch = _service.Create(conversation.ID, request);
        var dbFetch = Chat.Include(c => c.Conversation).First(c => c.ID == fetch.ID);
        
        Assert.Equivalent(request, fetch);
        Assert.Equal(conversation.ID, fetch.Conversation.ID);
        Assert.Equivalent(request, dbFetch);
        Assert.Equal(conversation.ID, dbFetch.Conversation.ID);
    }

    [Fact]
    public void Create_ChatAddedToConversation(){
        var request = AutoFaker.Generate<ChatDTO.Request>();
        var db = Db();
        var world = WorldFactory.CreateInstance(db);
        var conversation = ConversationFactory.Create(db, world);
        var fetch = _service.Create(conversation.ID, request);
        var dbFetch = Conversation.Include(c => c.Chats).First(c => c.ID == conversation.ID);

        Assert.Contains(dbFetch.Chats, c => c.ID == fetch.ID);
    }
    
    [Fact]
    public void Create_InvalidIDThrows(){
        Assert.Throws<KeyNotFoundException>(()=> _service.Create(_faker.Random.Guid(), AutoFaker.Generate<ChatDTO.Request>()));  
    }

    // =======================================
    //                  GET              
    // =======================================

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(5)]
    public void Get_RetrieveRequested(int index){
        var db = Db();
        var world = WorldFactory.CreateInstance(db);
        var conversation = ConversationFactory.Create(db, world);
        var chats = Enumerable.Range(0, 10).Select(_ => ChatFactory.Create(db, conversation)).ToList();
        var fetch = _service.Get(chats[index].ID);

        Assert.EquivalentWithExclusions(chats[index], fetch, x => x.Conversation);
        Assert.EquivalentWithExclusions(chats[index].Conversation, fetch.Conversation,
                                        x => x.Chats,
                                        x => x.World!.Conversations,
                                        //Conversation should not hold Narrator object
                                        x => x.World!.Narrator);
    }

    [Fact]
    public void Get_DoesNotIncludeNarrator(){
        var db = Db();
        var world = WorldFactory.CreateInstance(db);
        var conversation = ConversationFactory.Create(db, world);
        var chat = ChatFactory.Create(db, conversation);

        var fetch = _service.Get(chat.ID);
        Assert.Null(fetch.Conversation.World.Narrator);
    }

    [Fact]
    public void Get_InvalidIDReturnNull(){
        var fetch = _service.Get(_faker.Random.Guid());
        Assert.Null(fetch);
    }

    // =======================================
    //                 UPDATE              
    // =======================================

    [Fact]
    public void Update_SuccessfullChangeMessage(){
        var db = Db();
        var world = WorldFactory.CreateInstance(db);
        var conversation = ConversationFactory.Create(db, world);
        var chat = ChatFactory.Create(db, conversation);
        string newMessage = "";
        do{
            newMessage = _faker.Lorem.Word();
        } while(newMessage == chat.Content);

        var fetch = _service.Update(chat.ID, newMessage);
        var dbFetch = Chat.Find(chat.ID);
        
        Assert.Equal(newMessage, fetch.Content);
        Assert.Equal(newMessage, dbFetch.Content);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(5)]
    public void Update_OnlyRequestedMessageIsChanged(int index){
        var db = Db();
        var world = WorldFactory.CreateInstance(db);
        var conversation = ConversationFactory.Create(db, world);
        var chats = Enumerable.Range(0, 10).Select(_ => ChatFactory.Create(db, conversation)).ToList();

        var newMessage = "";
        do{
            newMessage = _faker.Lorem.Word();
        }while(chats.Any((c)=> c.Content == newMessage));

        _service.Update(chats[index].ID, newMessage);
        
        var expected = chats.Where(c => c.ID != chats[index].ID).ToList();
        var actual = Chat.Include(c => c.Conversation).Where(c => c.ID != chats[index].ID).ToList();

        Assert.EquivalentWithExclusions(expected, actual, "Conversation");
    }

    [Fact]
    public void Update_ReturnNullOnInvalidID(){
        Assert.Null(_service.Get(_faker.Random.Guid()));
    }

    // =======================================
    //                 DELETE              
    // =======================================

    [Fact]
    public void Delete_DeletesChat(){
        var db = Db();
        var world = WorldFactory.CreateInstance(db);
        var conversation = ConversationFactory.Create(db, world);
        var chat = ChatFactory.Create(db, conversation);

        _service.Delete(chat.ID);

        var dbFetch = Chat.Find(chat.ID);
        Assert.Null(dbFetch);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(5)]
    public void Delete_DeleteOnlyRequested(int index){
        var db = Db();
        var world = WorldFactory.CreateInstance(db);
        var conversation = ConversationFactory.Create(db, world);
        var chats = Enumerable.Range(0, 10).Select(_ => ChatFactory.Create(db, conversation)).ToList();
        _service.Delete(chats[index].ID);

        var actual = chats.Where(c => c.ID != chats[index].ID).ToList();
        var expected = Chat.ToList();

        Assert.EquivalentWithExclusions(actual, expected, "Conversation");
    }

    [Fact]
    public void Delete_DoesNothingOnInvalidID(){
        var exceptions = Record.Exception(() => _service.Delete(_faker.Random.Guid()));
        Assert.Null(exceptions);
    }
}
