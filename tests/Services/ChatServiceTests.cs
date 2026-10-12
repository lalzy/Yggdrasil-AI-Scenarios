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
using Yggdrasil.Models.Enums;
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

    [Fact]
    public void Create_CreatesChat(){
        var request = AutoFaker.Generate<ChatDTO.Request>();
        var conversation = ConversationFactory.Create(Db());
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
        var conversation = ConversationFactory.Create(Db());
        var fetch = _service.Create(conversation.ID, request);
        var dbFetch = Conversation.Include(c => c.Chats).First(c => c.ID == conversation.ID);

        Assert.Contains(dbFetch.Chats, c => c.ID == fetch.ID);
    }
    
    [Fact]
    public void Create_InvalidIDThrows(){
        Assert.Throws<KeyNotFoundException>(()=> _service.Create(_faker.Random.Guid(), AutoFaker.Generate<ChatDTO.Request>()));  
    }

    [Fact]
    public void Get_RetrieveRequested(){
        var conversation = ConversationFactory.Create(Db());
        var chat = ChatFactory.Create(Db(), conversation);
        var fetch = _service.Get(chat.ID);

        Assert.Equivalent(chat, fetch);
    }
}
